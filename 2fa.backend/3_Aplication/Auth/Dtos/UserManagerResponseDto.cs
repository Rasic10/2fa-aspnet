using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Aplication.Auth.Dtos
{
    public class UserManagerResponseDto
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public DateTime? ExpireDate { get; set; }
        public bool Is2StepVerificationRequired { get; set; }
        public string ProviderFor2FA { get; set; }
    }
}
