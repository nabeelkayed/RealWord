using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Models
{
    public class ProfileDto
    {
        public string username { get; set; }
        public string bio { get; set; }
        public string image { get; set; }
        public bool following { get; set; }
    }
}
