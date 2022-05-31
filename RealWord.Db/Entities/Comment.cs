using System;
using System.Collections.Generic;
using System.Text;

namespace RealWord.Db.Entities
{
    public class Comment
    {
        public Guid CommentId { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guid ArticleId { get; set; }
        public Article Article { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}