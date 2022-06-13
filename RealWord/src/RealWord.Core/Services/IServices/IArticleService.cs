using System;
using System.Collections.Generic;
using RealWord.Utils.ResourceParameters;
using System.Threading.Tasks;
using RealWord.Core.Models;

namespace RealWord.Core.Services
{
    public interface IArticleService
    {
        Task<bool> ArticleExistsAsync(string slug);
        Task<bool> IsAuthorized(string slug);
        Task<Guid> GetArticleIdAsync(string slug); 
        Task<IEnumerable<ArticleDto>> GetArticlesAsync(ArticlesParameters articlesParameters);
        Task<IEnumerable<ArticleDto>> FeedArticleAsync(FeedArticlesParameters feedArticlesParameters);
        Task<ArticleDto> GetArticleAsync(string slug);
        Task<ArticleDto> CreateArticleAsync(ArticleForCreationDto articleForCreation);
        Task<ArticleDto> UpdateArticleAsync(string slug, ArticleForUpdateDto articleForUpdate);
        Task DeleteArticleAsync(string slug);
        Task<ArticleDto> FavoriteArticleAsync(string slug);
        Task<ArticleDto> UnFavoriteArticleAsync(string slug);
    }
}