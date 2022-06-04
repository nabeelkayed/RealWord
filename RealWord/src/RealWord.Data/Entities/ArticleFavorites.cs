using System;
using System.Collections.Generic;
using System.Text;

namespace RealWord.Data.Entities
{
    public class ArticleFavorites
    {
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }

        public Article Article { get; set; }
        public User User { get; set; }
    }
}
