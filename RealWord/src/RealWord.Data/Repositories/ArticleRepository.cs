using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealWord.Data.Entities;
using RealWord.Utils.ResourceParameters; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RealWord.Utils.Utils;
using System.Threading.Tasks;

namespace RealWord.Data.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly RealWordDbContext _context;

        public ArticleRepository(RealWordDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<bool> ArticleExistsAsync(string slug)
        {
            if (String.IsNullOrEmpty(slug))
            {
                throw new ArgumentNullException(nameof(slug));
            }

            bool articleExists = await _context.Articles.AnyAsync(a => a.Slug == slug);
            return articleExists;
        }
        public async Task<Article> GetArticleAsync(string slug)
        {
            if (String.IsNullOrEmpty(slug))
            {
                throw new ArgumentNullException(nameof(slug));
            }

            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
            return article;
        }
        public async Task<List<Article>> GetArticlesAsync(ArticlesParameters articlesParameters)
        {
            var articles = _context.Articles.AsQueryable();

            if (!string.IsNullOrEmpty(articlesParameters.Tag))
            {
                var tag = articlesParameters.Tag.Trim();
                var userfavarets = await _context.ArticleTags.Where(af => af.TagId == tag)
                                                       .Select(a => a.ArticleId)
                                                       .ToListAsync();
                articles = articles.Where(a => userfavarets.Contains(a.ArticleId));
            }
            if (!string.IsNullOrEmpty(articlesParameters.Author))
            {
                var authorname = articlesParameters.Author.Trim();
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == authorname);
                articles = articles.Where(a => a.UserId == user.UserId);
            }
            if (!string.IsNullOrEmpty(articlesParameters.Favorited))
            {
                var username = articlesParameters.Favorited.Trim();
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                var userfavarets = await _context.ArticleFavorites.Where(af => af.UserId == user.UserId)
                                                            .Select(a => a.ArticleId)
                                                            .ToListAsync();
                articles = articles.Where(a => userfavarets.Contains(a.ArticleId));
            }

            articles = articles.Skip(articlesParameters.Offset)
                               .Take(articlesParameters.Limit)
                               .Include(a => a.User)
                               .ThenInclude(a => a.Followers)
                               .Include(a => a.Tags)
                               .Include(a => a.Favorites)
                               .OrderByDescending(x => x.CreatedAt);

            return await articles.ToListAsync();
        }
        public async Task<List<Article>> GetFeedArticlesAsync(Guid currentUserId, FeedArticlesParameters feedArticlesParameters)
        {
            var userFollowings = await _context.UserFollowers.Where(uf => uf.FollowerId == currentUserId)
                                                       .Select(uf => uf.FolloweingId)
                                                       .ToListAsync();
            if (!userFollowings.Any())
            {
                throw new ArgumentNullException(nameof(userFollowings));
            }

            var feedArticles = await _context.Articles.Where(a => userFollowings.Contains(a.UserId))
                                                .Include(a => a.User)
                                                .ThenInclude(a => a.Followers)
                                                .Include(a => a.Tags)
                                                .Include(a => a.Favorites)
                                                .OrderByDescending(x => x.CreatedAt)
                                                .Skip(feedArticlesParameters.Offset)
                                                .Take(feedArticlesParameters.Limit)
                                                .ToListAsync();
            return feedArticles;
        }
        public void CreateArticle(Article article)
        {
            _context.Articles.Add(article);
        }
        public void UpdateArticle(Article updatedArticle, Article articleForUpdate)
        {
        }
        public void DeleteArticle(Article article)
        {
            _context.Articles.Remove(article);
        }
        public void FavoriteArticle(Guid currentUserId, Guid articleToFavoriteId)
        {
            var articleFavorite =
                new ArticleFavorites { ArticleId = articleToFavoriteId, UserId = currentUserId };
             _context.ArticleFavorites.Add(articleFavorite);
        }
        public void UnfavoriteArticle(Guid currentUserId, Guid articleToUnFavoriteId)
        {
            var articleFavorite =
                new ArticleFavorites { ArticleId = articleToUnFavoriteId, UserId = currentUserId };
            _context.ArticleFavorites.Remove(articleFavorite);
        }
        public async Task<bool> IsFavoritedAsync(Guid UserId, Guid articleId)
        {
            var isFavorited =
               await _context.ArticleFavorites.AnyAsync(af => af.UserId == UserId && af.ArticleId == articleId);
            return isFavorited;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
