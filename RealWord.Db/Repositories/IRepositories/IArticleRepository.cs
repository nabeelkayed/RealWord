using RealWord.Db.Entities;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RealWord.Db.Repositories
{
    public interface IArticleRepository
    {
        Article GetArticle(string slug);
        void CreateArticle(Article article); 
        void UpdateArticle(Article article);
        void DeleteArticle(Article article);
        void FavoriteArticle(User currUser, Article article);
        void UnFavoriteArticle(User currUser, Article article);
        List<Article> GetArticles(string tag, string author, string favorited, int limit, int offset);
        //List<Article> GetArticles(QueryString query);

        bool ArticleExists(string slug);
        void Save(); 
        List<Article> GetFeedArticles(User currUser,int limit, int offset);
    }
}