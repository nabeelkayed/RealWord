using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealWord.Db.Entities;
using RealWord.Utils.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RealWord.Utils.Utils;


namespace RealWord.Db.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly RealWordDbContext _context;

        public ArticleRepository(RealWordDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public bool ArticleExists(string slug)
        {
            if (String.IsNullOrEmpty(slug))
            {
                throw new ArgumentNullException(nameof(slug));
            }

            bool articleExists = _context.Articles.Any(a => a.Slug == slug);
            return articleExists;
        }
        public Article GetArticle(string slug)
        {
            if (String.IsNullOrEmpty(slug))
            {
                throw new ArgumentNullException(nameof(slug));
            }

            var article = _context.Articles.FirstOrDefault(a => a.Slug == slug);
            return article;
        }
        public List<Article> GetArticles(ArticlesParameters articlesParameters)
        {
            var articles = _context.Articles.AsQueryable();

            if (!string.IsNullOrEmpty(articlesParameters.tag))
            {
                var tag = articlesParameters.tag.Trim();
                var userfavarets = _context.ArticleTags.Where(af => af.TagId == tag)
                                                       .Select(a => a.ArticleId)
                                                       .ToList();
                articles = articles.Where(a => userfavarets.Contains(a.ArticleId));
            }
            if (!string.IsNullOrEmpty(articlesParameters.author))
            {
                var authorname = articlesParameters.author.Trim();
                var user = _context.Users.FirstOrDefault(u => u.Username == authorname);
                articles = articles.Where(a => a.UserId == user.UserId);
            }
            if (!string.IsNullOrEmpty(articlesParameters.favorited))
            {
                var username = articlesParameters.favorited.Trim();
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                
                var userfavarets = _context.ArticleFavorites.Where(af => af.UserId == user.UserId)
                                                            .Select(a => a.ArticleId)
                                                            .ToList();
                articles = articles.Where(a => userfavarets.Contains(a.ArticleId));
            }

            articles = articles.Skip(articlesParameters.offset)
                               .Take(articlesParameters.limit)
                               .Include(a => a.User)
                               .ThenInclude(a => a.Followers)
                               .Include(a => a.Tags)
                               .Include(a => a.Favorites)
                               .OrderByDescending(x => x.CreatedAt);

            return articles.ToList();
        }
        public List<Article> GetFeedArticles(Guid currentUserId, FeedArticlesParameters feedArticlesParameters)
        {
            var userFollowings = _context.UserFollowers.Where(uf => uf.FollowerId == currentUserId)
                                                       .Select(uf => uf.FolloweingId)
                                                       .ToList();
            if (!userFollowings.Any())
            {
                throw new ArgumentNullException(nameof(userFollowings));
            }

            var feedArticles = _context.Articles.Where(a => userFollowings.Contains(a.UserId))
                                                .Include(a => a.User)
                                                .ThenInclude(a => a.Followers)
                                                .Include(a => a.Tags)
                                                .Include(a => a.Favorites)
                                                .OrderByDescending(x => x.CreatedAt)
                                                .Skip(feedArticlesParameters.offset)
                                                .Take(feedArticlesParameters.limit).ToList();
            return feedArticles;
        } 
        public void CreateArticle(Article article, List<string> tagList)
        {
            _context.Articles.Add(article);

            if (tagList != null && tagList.Any())
            {
                var tags = _context.Tags.Select(t => t.TagId).ToList();//فحص تكرار التاجات ويين لازم يكون زي الإيميل والعنوان
                
                foreach (var tag in tagList)
                {
                    if (!tags.Contains(tag))
                    {
                        var tag1 = new Tag { TagId = tag };
                        _context.Tags.Add(tag1);
                    }

                    var ArticleTags = new ArticleTags { TagId = tag, ArticleId = article.ArticleId };
                    _context.ArticleTags.Add(ArticleTags);
                }
            }
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
        public bool IsFavorited(Guid UserId, Guid articleId)
        {
            var isFavorited =
                _context.ArticleFavorites.Any(af => af.UserId == UserId && af.ArticleId == articleId);
            return isFavorited;
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
