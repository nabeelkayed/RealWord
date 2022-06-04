using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RealWord.Data.Entities
{
    public class User
    {
        public User()
        {
            Favorites = new List<ArticleFavorites>();
            Followers = new List<UserFollowers>();
            Followerings = new List<UserFollowers>();
            Articles = new List<Article>();
            Comments = new List<Comment>();
        }

        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }

        public List<ArticleFavorites> Favorites { get; set; }
        public List<UserFollowers> Followers { get; set; }
        public List<UserFollowers> Followerings { get; set; }

        public List<Article> Articles { get; set; }
        public List<Comment> Comments { get; set; }
    }
}