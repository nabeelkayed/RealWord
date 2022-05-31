using RealWord.Db.Entities;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RealWord.Db.Repositories
{
    public interface IArticleRepository
    {
        Article GetArticle(string slug);
        public void CreateArticle(Article article, List<string> tagList);
        public Article UpdateArticle(User u,string slug, Article article);
        void DeleteArticle(Article article);
        bool FavoriteArticle(User currUser, Article article);
        bool UnFavoriteArticle(User currUser, Article article);
        public bool Isfave(User currUser, Article article);
        List<Article> GetArticles(string tag, string author, string favorited, int limit, int offset);
        //List<Article> GetArticles(QueryString query);

        bool ArticleExists(string slug);
        void Save(); 
        List<Article> GetFeedArticles(User currUser,int limit, int offset);
    }
}