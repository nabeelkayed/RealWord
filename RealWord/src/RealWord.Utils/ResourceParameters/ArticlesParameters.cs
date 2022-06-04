using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Utils.ResourceParameters
{
    public class ArticlesParameters
    {
        public string Tag { get; set; }
        public string Author { get; set; }
        public string Favorited { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; } = 0;
    }
}
