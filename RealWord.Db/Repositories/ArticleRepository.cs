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
                                                            .Select(a => a.ArticleId).ToList();
                articles = articles.Where(a => userfavarets.Contains(a.ArticleId));
            }
            if (!string.IsNullOrEmpty(articlesParameters.author))
            {
                var authorname = articlesParameters.author.Trim();
                var user = _context.Users.Where(u => u.Username == authorname).FirstOrDefault();
                articles = articles.Where(a => a.UserId == user.UserId);
            }
            if (!string.IsNullOrEmpty(articlesParameters.favorited))
            {
                var username = articlesParameters.favorited.Trim();
                var user = _context.Users.Where(u => u.Username == username).FirstOrDefault();

                var userfavarets = _context.ArticleFavorites.Where(af => af.UserId == user.UserId)
                                                            .Select(a => a.ArticleId).ToList();
                articles = articles.Where(a => userfavarets.Contains(a.ArticleId));
                //Articles = Articles.Include(a => a.Favorites.Where(a => a.User.Username == favorited));
            }
            //هل اذا كامنت المقالات فاضية بصير مشكلة عند take
            articles = articles.Skip(articlesParameters.offset)
                               .Take(articlesParameters.limit)
                               .Include(a => a.User)
                               .ThenInclude(a => a.Followerings)
                               .Include(a => a.Tags)
                               .Include(a => a.Favorites)
                               .OrderByDescending(x => x.CreatedAt);

            return articles.ToList();
        }
        public List<Article> GetFeedArticles(Guid currentUserId, FeedArticlesParameters feedArticlesParameters)
        {
            //أعدل كلمة folloing في كل مكان
            var userFollowings = _context.UserFollowers.Where(uf => uf.FollowerId == currentUserId)
                                                      .Select(uf => uf.FolloweingId).ToList();
            if (!userFollowings.Any())
            {
                throw new ArgumentNullException(nameof(userFollowings));
            }

            var feedArticles = _context.Articles.Where(a => userFollowings.Contains(a.UserId))
                                                .Include(a => a.User)
                                                .ThenInclude(a => a.Followerings)
                                                .Include(a => a.Tags)
                                                .Include(a => a.Favorites)
                                                .OrderByDescending(x => x.CreatedAt)
                                                .Skip(feedArticlesParameters.offset)
                                                .Take(feedArticlesParameters.limit).ToList();
            return feedArticles;
        } 
        public void CreateArticle(Article article, List<string> tagList)
        {
            var Slugs = _context.Articles.Select(a => a.Slug).ToList();
            if (Slugs.Contains(article.Slug))
            {
                throw new ArgumentNullException(nameof(article.Slug));
            }
            _context.Articles.Add(article);

            if (tagList != null && tagList.Any())
            {
                var tags = _context.Tags.Select(t => t.TagId).ToList();

                foreach (var tag in tagList)
                {
                    if (!tags.Contains(tag))
                    {
                        _context.Tags.Add(new Tag { TagId = tag });
                    }
                    _context.ArticleTags.Add(new ArticleTags { TagId = tag, ArticleId = article.ArticleId });
                }
            }
        }
        public Article UpdateArticle(Guid currentUserId, string slug, Article articleForUpdate)
        {
            var updatedArticle = _context.Articles.Where(a => a.UserId == currentUserId)
                                                  .FirstOrDefault(a => a.Slug == slug);
            if (updatedArticle == null)
            {
                throw new ArgumentNullException(nameof(updatedArticle));
            }

            if (!string.IsNullOrWhiteSpace(articleForUpdate.Title))
            {
                updatedArticle.Title = articleForUpdate.Title;
                updatedArticle.Slug = Slug.GenerateSlug(articleForUpdate.Title);
            }

            if (!string.IsNullOrWhiteSpace(articleForUpdate.Description))
            {
                updatedArticle.Description = articleForUpdate.Description;
            }

            if (!string.IsNullOrWhiteSpace(articleForUpdate.Body))
            {
                updatedArticle.Body = articleForUpdate.Body;
            }

            updatedArticle.UpdatedAt = DateTime.Now;

            return updatedArticle;
        }
        public void DeleteArticle(Article article)
        {
            _context.Articles.Remove(article);
        }
        public bool FavoriteArticle(Guid currentUserId, Guid articleToFavoriteId)
        {
            bool isFavorited = Isfavorited(currentUserId, articleToFavoriteId);
            if (isFavorited)
            {
                return false;
            }

            var articleFavorite =
                new ArticleFavorites { ArticleId = articleToFavoriteId, UserId = currentUserId };
            _context.ArticleFavorites.Add(articleFavorite);

            return true;
        }
        public bool UnfavoriteArticle(Guid currentUserId, Guid articleToUnFavoriteId)
        {
            bool isUnfavorited = !Isfavorited(currentUserId, articleToUnFavoriteId);
            if (isUnfavorited)
            {
                return false;
            }

            var articleFavorite =
                new ArticleFavorites { ArticleId = articleToUnFavoriteId, UserId = currentUserId };

            _context.ArticleFavorites.Remove(articleFavorite);

            return true;
        }
        public bool Isfavorited(Guid UserId, Guid articleId)
        {
            var isfavorited =
                _context.ArticleFavorites.Any(af => af.UserId == UserId && af.ArticleId == articleId);

            return isfavorited;
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
