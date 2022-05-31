using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Models
{
    public class UserForCreationDto
    {
        public string email { get; set; }
        public string password { get; set; }
        public string username { get; set; }
    }
}
