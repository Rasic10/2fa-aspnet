using _3_Aplication.Auth.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Aplication.Auth
{
    public interface IUserService
    {
        Task<UserManagerResponseDto> RegisterUserAsync(UserRegisterDto model);
        Task<UserManagerResponseDto> LoginUserAsync(UserLoginDto model);
        Task<UserManagerResponseDto> TwoStepVerification(TwoFactorDto twoFactorDto);
    }
}
