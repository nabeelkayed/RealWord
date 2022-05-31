using RealWord.Db.Entities;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RealWord.Db.Repositories
{
    public interface IArticleRepository
    {
        bool ArticleExists(string Slug);
        List<Article> GetArticles(string Tag, string Author, string Favorited, int Limit, int Offset);
        List<Article> GetFeedArticles(User CurrentUser, int Limit, int Offset);
        Article GetArticle(string Slug);
        public void CreateArticle(Article Article, List<string> TagList);
        public Article UpdateArticle(User User,string Slug, Article Article);
        void DeleteArticle(Article Article);
        bool FavoriteArticle(User CurrentUser, Article Article);
        bool UnfavoriteArticle(User CurrentUser, Article Article); 
        public bool Isfavorite(User CurrentUser, Article Article);
        //List<Article> GetArticles(QueryString query);
        void Save();  
    }
}