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
        public bool ArticleExists(string Slug)
        {
            if (String.IsNullOrEmpty(Slug))///هذا الفحص لازم نفعمله على كل انبت
            {
                throw new ArgumentNullException(nameof(Slug));
            }

            return _context.Articles.Any(a => a.Slug == Slug);
        }
        public Article GetArticle(string Slug)
        {
            return _context.Articles.FirstOrDefault(a => a.Slug == Slug);
        }
        public void CreateArticle(Article Article, List<string> tagList)
        {
            var TimeStamp = DateTime.Now;
            Article.CreatedAt = TimeStamp;
            Article.UpdatedAt = TimeStamp;

            var AllSlugs = _context.Articles.Select(s => s.Slug).ToList();
            //فحص اذا كان العنوان مكرر
            if (!AllSlugs.Contains(Article.Slug))
            {
                _context.Articles.Add(Article);
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
                _context.ArticleTags.Add(new ArticleTags { TagId = t, ArticleId = Article.ArticleId });
            }
        }
        public Article UpdateArticle(User u, string slug, Article article)
        {
            //لازم اتأكد انها المقال تبعته لأنه ما بقدر يعدل على أي مقال 
            var a = _context.Articles.Where(a => a.UserId == u.UserId).FirstOrDefault(a => a.Slug == slug);

            if (!string.IsNullOrWhiteSpace(article.Title))
            {
                a.Title = article.Title;
                // a.Slug= Slug.GenerateSlug(articleEntity.Title);
            }

            if (!string.IsNullOrWhiteSpace(article.Description))
            {
                a.Description = article.Description;
            }

            if (!string.IsNullOrWhiteSpace(article.Body))
            {
                a.Body = article.Body;
            }

            a.UpdatedAt = DateTime.Now;

            return a;
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
            if (Isfavorite(currUser, article))
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

        public bool UnfavoriteArticle(User CurrentUser, Article Article)
        {
            if (!Isfavorite(CurrentUser, Article))
            {
                return false;
            }

            var ArticleFavorite = new ArticleFavorites { ArticleId = Article.ArticleId, UserId = CurrentUser.UserId };
            _context.ArticleFavorites.Remove(ArticleFavorite);

            return true;
        }

        public bool Isfavorite(User CurrentUser, Article Article)
        {
            return _context.ArticleFavorites.Any(af => af.UserId == CurrentUser.UserId
                       && af.ArticleId == Article.ArticleId);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
