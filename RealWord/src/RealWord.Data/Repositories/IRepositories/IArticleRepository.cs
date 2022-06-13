using RealWord.Data.Entities;
using System;
using System.Collections.Generic;
using RealWord.Utils.ResourceParameters;
using System.Threading.Tasks;

namespace RealWord.Data.Repositories
{
    public interface IArticleRepository
    {
        Task<bool> ArticleExistsAsync(string slug);
        Task<Article> GetArticleAsync(string slug);
        Task<List<Article>> GetArticlesAsync(ArticlesParameters articlesParameters);
        Task<List<Article>> GetFeedArticlesAsync(Guid currentUserId, FeedArticlesParameters feedArticlesParameters);
        Task CreateArticleAsync(Article article);
        void UpdateArticle(Article updatedArticle, Article articleForUpdate);
        void DeleteArticle(Article article);
        Task FavoriteArticleAsync(Guid currentUserId, Guid articleToFavoriteId);
        void UnfavoriteArticle(Guid currentUserId, Guid articleToUnfavoriteId);
        Task<bool> IsFavoritedAsync(Guid UserId, Guid articleId);
        Task SaveChangesAsync();
    }
}