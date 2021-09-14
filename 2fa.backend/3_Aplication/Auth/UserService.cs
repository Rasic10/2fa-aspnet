using _3_Aplication.Auth.Dtos;
using _3_Aplication.Auth.SendGrid;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace _3_Aplication.Auth
{
    public class UserService : IUserService
    {
        private UserManager<IdentityUser> _userManager;
        private IConfiguration _configuration;
        private IMailService _mailSevice;

        public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration, IMailService mailSevice)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailSevice = mailSevice;
        }

        public async Task<UserManagerResponseDto> LoginUserAsync(UserLoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new UserManagerResponseDto
                {
                    Message = "There is no user with that email address",
                    IsSuccess = false
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return new UserManagerResponseDto
                {
                    Message = "Invalid password",
                    IsSuccess = false
                };
            }

            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return await GenerateOTPFor2StepVerification(user);
            }

            var token = GenerateJWTToken(model.Email, user.Id);
            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponseDto
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }

        public async Task<UserManagerResponseDto> RegisterUserAsync(UserRegisterDto model)
        {
            if (model == null)
            {
                throw new NullReferenceException("Register model is null!");
            }

            if (model.Password != model.ConfirmPassword)
            {
                return new UserManagerResponseDto
                {
                    Message = "confirm password doesn't match the password!",
                    IsSuccess = false,
                };
            }

            var identityUser = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                return new UserManagerResponseDto
                {
                    Message = "User created successfully!",
                    IsSuccess = true
                };
            }

            return new UserManagerResponseDto
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponseDto> TwoStepVerification(TwoFactorDto twoFactorDto)
        {
            var user = await _userManager.FindByEmailAsync(twoFactorDto.Email);
            
            if (user == null)
            {
                return new UserManagerResponseDto
                {
                    Message = "Something goes wrong",
                    IsSuccess = false
                };
            }

            var validVerification = await _userManager.VerifyTwoFactorTokenAsync(user, twoFactorDto.Provider, twoFactorDto.Token);

            if (!validVerification)
            {
                return new UserManagerResponseDto
                {
                    Message = "Invalid token verification",
                    IsSuccess = false
                };
            }

            var token = GenerateJWTToken(twoFactorDto.Email, user.Id);
            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponseDto
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }

        private JwtSecurityToken GenerateJWTToken(string email, string id)
        {
            var claims = new[]
            {
                new Claim("Email", email),
                new Claim("ID", id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return token;
        }

        private async Task<UserManagerResponseDto> GenerateOTPFor2StepVerification(IdentityUser user)
        {
            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);

            if (!providers.Contains("Email"))
            {
                return new UserManagerResponseDto 
                { 
                    Message = "Invalid 2-Step Verification Provider.",
                    IsSuccess = false
                };
            }

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
          
            await _mailSevice.SendEmailAsync(user.Email, "Two factor authentication", "<h1> Hey, your token is: " + token + " </h1>");

            return new UserManagerResponseDto 
            {
                Message = "Two factor authentication is enabled",
                IsSuccess = true,
                Is2StepVerificationRequired = true, 
                ProviderFor2FA = "Email" 
            };
        }
    }
}
