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

            return _context.Articles.Any(a => a.Slug == Slug);//هل لازم أفصل الجملة عن الريتيرن
        }
        public Article GetArticle(string Slug)
        {
            return _context.Articles.FirstOrDefault(a => a.Slug == Slug);
        }
        public void CreateArticle(Article Article, List<string> TagList)
        {
            var Slugs = _context.Articles.Select(a => a.Slug).ToList();
            //فحص اذا كان العنوان مكرر
            if (!Slugs.Contains(Article.Slug))
            {
                _context.Articles.Add(Article);
            }
            else
            {

            }

            var Tags = _context.Tags.Select(t => t.TagId).ToList();
            foreach (var Tag in TagList)
            {
                if (!Tags.Contains(Tag))
                {
                    _context.Tags.Add(new Tag { TagId = Tag });
                }
                _context.ArticleTags.Add(new ArticleTags { TagId = Tag, ArticleId = Article.ArticleId });
            }
        }
        public Article UpdateArticle(User User, string Slug, Article Article)
        {
            //لازم اتأكد انها المقال تبعته لأنه ما بقدر يعدل على أي مقال 
            var UpdatedArticle = _context.Articles.Where(a => a.UserId == User.UserId).FirstOrDefault(a => a.Slug == Slug);

            if (!string.IsNullOrWhiteSpace(Article.Title))
            {
                UpdatedArticle.Title = Article.Title;
                // a.Slug= Slug.GenerateSlug(articleEntity.Title);
            }

            if (!string.IsNullOrWhiteSpace(Article.Description))
            {
                UpdatedArticle.Description = Article.Description;
            }

            if (!string.IsNullOrWhiteSpace(Article.Body))
            {
                UpdatedArticle.Body = Article.Body;
            }

            UpdatedArticle.UpdatedAt = DateTime.Now;

            return UpdatedArticle;
        }
        public void DeleteArticle(Article Article)
        {
            _context.Articles.Remove(Article);
        }

        public List<Article> GetArticles(string Tag, string Author, string Favorited, int Limit, int Offset)
        {
            var user = new User();
            var Articles = _context.Articles.ToList();//.AsQueryable();

            if (!string.IsNullOrEmpty(Tag))
            {
                // Articles = Articles.Where(a => a.Tags.Contains(tag)).ToList();

                // Articles = Articles.Include(a => a.Tags.Where(a => a.TagId == tag));
            }
            if (!string.IsNullOrEmpty(Author))
            {
                //بدنا اليوزر الحالي
                //بعدين بدنا كل المقالات الهو كاتبها
                Articles = Articles.Where(a => a.UserId == user.UserId).ToList();
                //Articles = Articles.Include(a => a.User.Username == author);
            }
            if (!string.IsNullOrEmpty(Favorited))
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
        public List<Article> GetFeedArticles(User CurrentUser, int Limit, int Offset)
        {
            var Articles = _context.Articles.AsQueryable();
            //مين الناس الي متابعهم اليوزر الحالي 
            //اجيب مقالات كل واحد فيهم
            //أعدل كلمة folloing في كل مكان
            var UseFollowings = _context.UserFollowers.Where(u => u.FollowerId == CurrentUser.UserId).Select(a => a.FolloweingId).ToList();
            // أفحص اذا ما كان فيه ولا مقال أو ولا متابع

            var FeedArticles = _context.Articles.Where(a => UseFollowings.Contains(a.UserId)).OrderByDescending(x => x.CreatedAt)
                .Skip(Offset).Take(Limit).ToList();
            return FeedArticles;
            // xx=xx.Skip(offset).Take(limit);
            /* foreach(var f in x)
             {
                 var g = _context.Articles.Where(a => a.UserId==f);
             }*/
            // Articles = Articles.Skip(offset).Take(limit);

            // Articles = Articles.Where(a => a.User == currUser).Skip(offset).Take(limit);

            // return Articles.ToList();

        }
        public bool FavoriteArticle(User CurrentUser, Article Article)//هل يرجع مقال والا لا
        {
            if (Isfavorite(CurrentUser, Article))
            {
                return false;
            }

            var ArticleFavorite = new ArticleFavorites { ArticleId = Article.ArticleId, UserId = CurrentUser.UserId };
            _context.ArticleFavorites.Add(ArticleFavorite);

            return true;
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
