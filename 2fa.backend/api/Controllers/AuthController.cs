using _3_Aplication.Auth;
using _3_Aplication.Auth.Dtos;
using _3_Aplication.Auth.SendGrid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private IMailService _mailSevice;

        public AuthController(IUserService userService, IMailService mailService)
        {
            _userService = userService;
            _mailSevice = mailService;
        }

        // api/auth/register
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAsync(model);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid"); // status code: 400
        }

        // api/auth/login
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);

                if (result.IsSuccess)
                {
                    //await _mailSevice.SendEmailAsync(model.Email, "New login", "<h1> Hey, new login to your account noticed</h1>" +
                    //    "<p>New login to your account at " + DateTime.Now + "</p>");

                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid"); // status code: 400
        }

        // api/auth/twostepverification
        [HttpPost("TwoStepVerification")]
        public async Task<IActionResult> TwoStepVerification([FromBody] TwoFactorDto twoFactorDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.TwoStepVerification(twoFactorDto);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid"); // status code: 400
        }
    }
}
