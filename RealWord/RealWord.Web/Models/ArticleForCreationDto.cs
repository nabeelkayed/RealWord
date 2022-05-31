using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Models
{
    public class ArticleForCreationDto
    {
        public string title { get; set; }
        public string description { get; set; }
        public string body { get; set; }
        public List<string> tagList { get; set; }
    }
}