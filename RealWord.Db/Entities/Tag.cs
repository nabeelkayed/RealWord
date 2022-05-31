using System;
using System.Collections.Generic;
using System.Text;

namespace RealWord.Db.Entities
{
    public class Tag
    {
        public Tag()
        {
            Articles = new List<ArticleTags>();
        }

        public string TagId { get; set; }

        public List<ArticleTags> Articles { get; set; }
    }  
}
