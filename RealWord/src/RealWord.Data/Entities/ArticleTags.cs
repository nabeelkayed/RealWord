using System;
using System.Collections.Generic;
using System.Text;

namespace RealWord.Data.Entities
{
    public class ArticleTags
    {
        public Guid ArticleId { get; set; }
        public string TagId { get; set; }

        public Article Article { get; set; } 
        public Tag Tag { get; set; } 
    }
}
