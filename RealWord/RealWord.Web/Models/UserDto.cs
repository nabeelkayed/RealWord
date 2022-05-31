using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Models
{
    public class UserDto
    {
        public string email { get; set; }
        public string token { get; set; }
        public string username { get; set; }
        public string bio { get; set; }
        public string image { get; set; }
    }
}
