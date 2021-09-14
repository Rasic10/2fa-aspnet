using System;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class ApplicationUser
    {
        public int UserId { get; set; }
        public string FristName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
