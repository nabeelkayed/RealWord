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
using RealWord.Utils.Utils;
using System.Security.Claims;
using RealWord.Utils.ResourceParameters;

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
        public ActionResult<IEnumerable<ArticleDto>> GetArticles(ArticlesParameters articlesParameters)
        {
            var articles = _IArticleRepository.GetArticles(articlesParameters);

            if (articles == null)
            {
                ///need to compleate
            }

            var currentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (currentUsername != null)
            {

            }

            var articlesToReturn = _mapper.Map<IEnumerable<ArticleDto>>(articles);
            int articlesCount = articles.Count();

            return Ok(new { articles = articlesToReturn, articlesCount = articlesCount });
        }

        [HttpGet("feed")]
        public ActionResult<IEnumerable<ArticleDto>> FeedArticle(FeedArticlesParameters feedArticlesParameters)
        {
            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var CurrentUser = _IUserRepository.GetUser(CurrentUsername);


            var articles = _IArticleRepository.GetFeedArticles(CurrentUser.UserId, feedArticlesParameters);

            if (!articles.Any())
            {
                //the followings have no articles
            }

            var articlesToReturn = _mapper.Map<IEnumerable<ArticleDto>>(articles);
            int articlesCount = articles.Count();

            return Ok(new { articles = articlesToReturn, articlesCount = articlesCount });

            //أضيف fave and vave count
            /*  foreach(var c in x)
              {
                  c.favorited = _IArticleRepository.Isfave(currUser, c.slug);
              }*/
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

            var articleToReturn = _mapper.Map<ArticleDto>(article);

            return Ok(new { article = articleToReturn });
        }

        [HttpPost]
        public ActionResult<ArticleDto> CreateArticle(ArticleForCreationDto articleForCreation)
        {
            var articleEntityForCreation = _mapper.Map<Article>(articleForCreation);
            articleEntityForCreation.Slug = Slug.GenerateSlug(articleEntityForCreation.Title);

            var currentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = _IUserRepository.GetUser(currentUsername);
            articleEntityForCreation.UserId = currentUser.UserId;

            var timeStamp = DateTime.Now;
            articleEntityForCreation.CreatedAt = timeStamp;
            articleEntityForCreation.UpdatedAt = timeStamp;

            _IArticleRepository.CreateArticle(articleEntityForCreation, articleForCreation.TagList);
            _IArticleRepository.Save();

            var ArticleToReturn = _mapper.Map<ArticleDto>(articleEntityForCreation);
            ArticleToReturn.Favorited = _IArticleRepository
                                       .Isfavorited(currentUser.UserId, articleEntityForCreation.ArticleId);

            return Ok(new { article = ArticleToReturn });
        }

        [HttpPut("{slug}")]
        public ActionResult<ArticleDto> UpdateArticle(string slug, ArticleForUpdateDto articleForUpdate)
        {
            var article = _IArticleRepository.GetArticle(slug);
            if (article == null)
            {
                return NotFound();
            }

            var currentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = _IUserRepository.GetUser(currentUsername);

            var articleEntityForUpdate = _mapper.Map<Article>(articleForUpdate);

            var updatedArticle = _IArticleRepository.UpdateArticle(currentUser.UserId, slug, articleEntityForUpdate);
            _IArticleRepository.Save();

            var articleToReturn = _mapper.Map<ArticleDto>(updatedArticle);

            return Ok(new { article = articleToReturn });
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

            var currentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = _IUserRepository.GetUser(currentUsername);


            var articleFavorited = _IArticleRepository.FavoriteArticle(currentUser.UserId, article.ArticleId);
            if (articleFavorited)
            {
                _IArticleRepository.Save();

                var articleToReturn = _mapper.Map<ArticleDto>(article);
                articleToReturn.Favorited = true;

                return Ok(new { article = articleToReturn });
            }

            return BadRequest($"You already favorite the article with slug {slug}");
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

            var currentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = _IUserRepository.GetUser(currentUsername);

            var articleUnfavorited = _IArticleRepository.UnfavoriteArticle(currentUser.UserId, article.ArticleId);
            if (articleUnfavorited)
            {
                _IArticleRepository.Save();

                var articleToReturn = _mapper.Map<ArticleDto>(article);
                articleToReturn.Favorited = false;

                return Ok(new { article = articleToReturn });
            }
            return BadRequest($" You aren't favorite the article with slug {slug}");
        }

    }
}