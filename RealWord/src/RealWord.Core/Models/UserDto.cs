using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Core.Models
{
    public class UserDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; } 
    }
}
