using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealWord.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            return _context.Articles.Any(a => a.Slug == slug);
        }
        public Article GetArticle(string slug)
        {
            return _context.Articles.FirstOrDefault(a => a.Slug == slug);
        }
        public void CreateArticle(Article article, List<string> tagList)
        {
            var timestamp = DateTime.Now;
            article.CreatedAt = timestamp;
            article.UpdatedAt = timestamp;

            var allslugs = _context.Articles.Select(s => s.Slug).ToList();
            //فحص اذا كان العنوان مكرر
            if (!allslugs.Contains(article.Slug))
            {
                _context.Articles.Add(article);
            }
            else
            {

            }

            var alltags = _context.Tags.Select(s => s.TagId).ToList();
            foreach (var t in tagList)
            {
                if (!alltags.Contains(t))
                {
                    _context.Tags.Add(new Tag { TagId = t });
                }
                _context.ArticleTags.Add(new ArticleTags { TagId = t, ArticleId = article.ArticleId });
            }
            /* var timestamp = DateTime.Now;
             _context.Entry(article).Property("CreatedAt").CurrentValue = timestamp;
             _context.Entry(article).Property("UpdatedAt").CurrentValue = timestamp;*/
        }
        public void UpdateArticle(Article article)
        {
            //_context.Entry(article).Property("UpdatedAt").CurrentValue = DateTime.Now;
        }
        public void DeleteArticle(Article article)
        {
            _context.Articles.Remove(article);
        }

        public List<Article> GetArticles(string tag, string author, string favorited, int limit, int offset)
        {
            var user = new User();
            var Articles = _context.Articles.ToList();//.AsQueryable();

            if (!string.IsNullOrEmpty(tag))
            {
                // Articles = Articles.Where(a => a.Tags.Contains(tag)).ToList();

                // Articles = Articles.Include(a => a.Tags.Where(a => a.TagId == tag));
            }
            if (!string.IsNullOrEmpty(author))
            {
                //بدنا اليوزر الحالي
                //بعدين بدنا كل المقالات الهو كاتبها
                Articles = Articles.Where(a => a.UserId == user.UserId).ToList();
                //Articles = Articles.Include(a => a.User.Username == author);
            }
            if (!string.IsNullOrEmpty(favorited))
            {
                // بدنا اليوزر الحالي
                //بعدين بدنا نجيب كل المقالات الي اليوزر الحالي متابعها
                Articles = Articles.Where(a => a.UserId == user.UserId).ToList();
                //Articles = Articles.Include(a => a.Favorites.Where(a => a.User.Username == favorited));
            }

            //أفحص اذا ما كان فيه ولا مقال
            // Articles = Articles.Skip(offset).Take(limit);
           // .OrderByDescending(x => x.CreatedAt) الترتيب مهم

            return Articles.ToList();
        }
        public List<Article> GetFeedArticles(User curruser, int limit, int offset)
        {
            var Articles = _context.Articles.AsQueryable();
            //مين الناس الي متابعهم اليوزر الحالي 
            //اجيب مقالات كل واحد فيهم
            var x = _context.UserFollowers.Where(u => u.FollowerId == curruser.UserId).Select(a => a.FolloweingId).ToList();
            // أفحص اذا ما كان فيه ولا مقال أو ولا متابع

            var xx = _context.Articles.Where(a => x.Contains(a.UserId)).OrderByDescending(x => x.CreatedAt)
                .Skip(offset).Take(limit).ToList();
            return xx;
            // xx=xx.Skip(offset).Take(limit);
            /* foreach(var f in x)
             {
                 var g = _context.Articles.Where(a => a.UserId==f);
             }*/
            // Articles = Articles.Skip(offset).Take(limit);

            // Articles = Articles.Where(a => a.User == currUser).Skip(offset).Take(limit);

            // return Articles.ToList();

        }
        public bool FavoriteArticle(User currUser, Article article)//هل يرجع مقال والا لا
        {
            bool ff = Isfave(currUser, article);
            if (ff)
            {
                return false;
            }
            var ArticleFavorite = new ArticleFavorites { ArticleId = article.ArticleId, UserId = currUser.UserId };
            _context.ArticleFavorites.Add(ArticleFavorite);
            return true;
            /*var currUser = _context.Users.Find("");
            var ArticleFavorite = new ArticleFavorites { ArticleId = article.ArticleId, UserId = currUser.UserId };
            _context.ArticleFavorites.Add(ArticleFavorite);*/
        }

        public bool UnFavoriteArticle(User currUser, Article article)
        {
            bool ff = Isfave(currUser, article);
            if (!ff)
            {
                return false;
            }
            var ArticleFavorite = new ArticleFavorites { ArticleId = article.ArticleId, UserId = currUser.UserId };
            _context.ArticleFavorites.Remove(ArticleFavorite);
            return true;
        }

        public bool Isfave(User currUser, Article article)
        {
            return _context.ArticleFavorites.Any(af => af.UserId == currUser.UserId
                       && af.ArticleId == article.ArticleId);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
