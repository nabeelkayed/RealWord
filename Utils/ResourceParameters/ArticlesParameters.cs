using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Utils.ResourceParameters
{
    public class ArticlesParameters
    {
        public string tag { get; set; }
        public string author { get; set; }
        public string favorited { get; set; }
        public int limit { get; set; } = 20;
        public int offset { get; set; } = 0;
    }
}
