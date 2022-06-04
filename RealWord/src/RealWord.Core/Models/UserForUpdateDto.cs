using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Core.Models
{
    public class UserForUpdateDto
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public string Bio { get; set; }
    }
}
