using System;
using System.Collections.Generic;
using System.Text;

namespace RealWord.Db.Entities
{
    public class Article
    {
        public Article()
        {
            Tags = new List<ArticleTags>();
            Favorites = new List<ArticleFavorites>();
            Comments = new List<Comment>();
        }

        public Guid ArticleId { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<ArticleTags> Tags { get; set; }
        public List<ArticleFavorites> Favorites { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public List<Comment> Comments { get; set; }


    }
}

