using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Aplication.Auth.Dtos
{
    public class TwoFactorDto
    {
        public string Email { get; set; }
        public string Provider { get; set; }
        public string Token { get; set; }
    }
}
