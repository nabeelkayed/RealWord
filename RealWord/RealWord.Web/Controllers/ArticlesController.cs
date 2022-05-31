using AutoMapper;
using RealWord.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealWord.Db.Repositories;
using RealWord.Db.Entities;
using Microsoft.AspNetCore.Authorization;
using RealWord.Web.Utils;
using System.Security.Claims;

namespace RealWord.Web.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleRepository _IArticleRepository;
        private readonly ICommentRepository _ICommentRepository;
        private readonly IUserRepository _IUserRepository;

        private readonly IMapper _mapper;

        public ArticlesController(IArticleRepository articleRepository,
            ICommentRepository commentRepository, IUserRepository userRepository,
            IMapper mapper)
        {
            _IArticleRepository = articleRepository ??
                throw new ArgumentNullException(nameof(articleRepository));
            _IUserRepository = userRepository ??
                throw new ArgumentNullException(nameof(userRepository));
            _ICommentRepository = commentRepository ??
               throw new ArgumentNullException(nameof(commentRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<IEnumerable<ArticleDto>> GetArticles(string tag, string author,
                                    string favorited, int limit = 20, int offset = 0)
        {

            var articles = _IArticleRepository.GetArticles(tag, author, favorited, limit, offset);
            int articlesCount = articles.Count();
            return Ok(new { articles = _mapper.Map<IEnumerable<ArticleDto>>(articles), articlesCount = articlesCount });
        }

        [HttpGet("feed")]
        public ActionResult<IEnumerable<ArticleDto>> FeedArticle(int limit = 20, int offset = 0)
        {
            var currUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var currUser = _IUserRepository.GetUser(currUsername);


            var articles = _IArticleRepository.GetFeedArticles(currUser, limit, offset);
            int articlesCount = articles.Count();
            return Ok(new { articles = articles, articlesCount = articlesCount });
        }

        [AllowAnonymous]
        [HttpGet("{slug}")]
        public ActionResult<ArticleDto> GetArticle(string slug)
        {
            var article = _IArticleRepository.GetArticle(slug);

            if (article == null)
            {
                return NotFound();
            }
            return Ok(new { article = _mapper.Map<ArticleDto>(article) });

        }

        [HttpPost]
        public ActionResult<ArticleDto> CreateArticle(ArticleForCreationDto article)
        {
            var articleEntity = _mapper.Map<Article>(article);

            articleEntity.Slug = Slug.GenerateSlug(articleEntity.Title);

            var username = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var CurrentUser = _IUserRepository.GetUser(username);

            articleEntity.UserId = CurrentUser.UserId;
            _IArticleRepository.CreateArticle(articleEntity);
            _IArticleRepository.Save();

            return Ok(new { article = _mapper.Map<ArticleDto>(articleEntity) });
        }

        [HttpPut("{slug}")]
        public ActionResult<ArticleDto> UpdateArticle(string slug, ArticleForUpdateDto articleforupdate)
        {
            var article = _IArticleRepository.GetArticle(slug);

            if (article == null)
            {
                return NotFound();
            }

            _mapper.Map(articleforupdate, article);

            _IArticleRepository.UpdateArticle(article);
            _IArticleRepository.Save();

            return Ok(new { article = _mapper.Map<ArticleDto>(articleforupdate) });
        }

        [HttpDelete("{slug}")]
        public IActionResult DeleteArticle(string slug)
        {
            var article = _IArticleRepository.GetArticle(slug);

            if (article == null)
            {
                return NotFound();
            }

            _IArticleRepository.DeleteArticle(article);
            _ICommentRepository.Save();

            return NoContent();
        }

        [HttpPost("{slug}/favorite")]
        public ActionResult<ArticleDto> FavoriteArticle(string slug)
        {
            var article = _IArticleRepository.GetArticle(slug);

            if (article == null)
            {
                return NotFound();
            }

            var currUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var currUser = _IUserRepository.GetUser(currUsername);
            ////

            var f = _IArticleRepository.FavoriteArticle(currUser, article);
            if (f)
            {
                _IArticleRepository.Save();
                var article1 = _mapper.Map<ArticleDto>(article);
                article1.favorited = true;
                return Ok(new { article = article1 });
            }
            return BadRequest($"You alredy favarte the user with slug {slug}");
             //var ff = _IUserRepository.FollowUser(currUser, user);
        }

        [HttpDelete("{slug}/favorite")]
        public IActionResult UnFavoriteArticle(string slug)
        {
            var article = _IArticleRepository.GetArticle(slug);

            if (article == null)
            {
                return NotFound();
            }

            var currUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var currUser = _IUserRepository.GetUser(currUsername);

            var f = _IArticleRepository.UnFavoriteArticle(currUser, article);
            if (f)
            {
                _IArticleRepository.Save();
                var article1 = _mapper.Map<ArticleDto>(article);
                article1.favorited = false;
                return Ok(new { article = article1 });
            }
            return BadRequest($"You alredy not favarte the article with slug {slug}");
        }

    }
}