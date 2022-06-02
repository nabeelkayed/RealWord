using RealWord.Db.Entities;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using RealWord.Utils.ResourceParameters;

namespace RealWord.Db.Repositories
{
    public interface IArticleRepository
    {
        bool ArticleExists(string slug);
        Article GetArticle(string slug);
        List<Article> GetArticles(ArticlesParameters articlesParameters);
        List<Article> GetFeedArticles(Guid currentUserId, FeedArticlesParameters feedArticlesParameters);
        void CreateArticle(Article article, List<string> tagList); 
        void UpdateArticle(Article updatedArticle, Article articleForUpdate);
        void DeleteArticle(Article article);
        void FavoriteArticle(Guid currentUserId, Guid articleToFavoriteId);
        void UnfavoriteArticle(Guid currentUserId, Guid articleToUnfavoriteId);
        bool IsFavorited(Guid UserId, Guid articleId);
        void Save();
    }
}