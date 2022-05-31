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
            var Articles = _IArticleRepository.GetArticles(tag, author, favorited, limit, offset);

            var ArticlesToReturn = _mapper.Map<IEnumerable<ArticleDto>>(Articles);
            int ArticlesCount = Articles.Count();

            return Ok(new { articles = ArticlesToReturn, articlesCount = ArticlesCount });
        }

        [HttpGet("feed")]
        public ActionResult<IEnumerable<ArticleDto>> FeedArticle(int limit = 20, int offset = 0)
        {
            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var CurrentUser = _IUserRepository.GetUser(CurrentUsername);


            var Articles = _IArticleRepository.GetFeedArticles(CurrentUser, limit, offset);

            var ArticlesToReturn = _mapper.Map<IEnumerable<ArticleDto>>(Articles);
            int ArticlesCount = Articles.Count();

            return Ok(new { articles = ArticlesToReturn, articlesCount = ArticlesCount });

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
            var Article = _IArticleRepository.GetArticle(slug);
            if (Article == null)
            {
                return NotFound();
            }

            var ArticleToReturn = _mapper.Map<ArticleDto>(Article);

            return Ok(new { article = ArticleToReturn });
        }

        [HttpPost]
        public ActionResult<ArticleDto> CreateArticle(ArticleForCreationDto ArticleForCreation)
        {
            var ArticleEntityForCreation = _mapper.Map<Article>(ArticleForCreation);
            ArticleEntityForCreation.Slug = Slug.GenerateSlug(ArticleEntityForCreation.Title);

            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var CurrentUser = _IUserRepository.GetUser(CurrentUsername);
            ArticleEntityForCreation.UserId = CurrentUser.UserId;

            _IArticleRepository.CreateArticle(ArticleEntityForCreation, ArticleForCreation.tagList);
            _IArticleRepository.Save();

            var ArticleToReturn = _mapper.Map<ArticleDto>(ArticleEntityForCreation);
            ArticleToReturn.favorited = _IArticleRepository.Isfavorite(CurrentUser, ArticleEntityForCreation);

            return Ok(new { article = ArticleToReturn });
        }

        [HttpPut("{slug}")]
        public ActionResult<ArticleDto> UpdateArticle(string slug, ArticleForUpdateDto ArticleForUpdate)
        {
            var Article = _IArticleRepository.GetArticle(slug);
            if (Article == null)
            {
                return NotFound();
            }

            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var CurrentUser = _IUserRepository.GetUser(CurrentUsername);

            var ArticleEntityForUpdate = _mapper.Map<Article>(ArticleForUpdate);

            var UpdatedArticle = _IArticleRepository.UpdateArticle(CurrentUser, slug, ArticleEntityForUpdate);
            _IArticleRepository.Save();

            var ArticleToReturn = _mapper.Map<ArticleDto>(UpdatedArticle);

            return Ok(new { article = ArticleToReturn });
        }

        [HttpDelete("{slug}")]
        public IActionResult DeleteArticle(string slug)
        {
            var Article = _IArticleRepository.GetArticle(slug);
            if (Article == null)
            {
                return NotFound();
            }

            _IArticleRepository.DeleteArticle(Article);
            _ICommentRepository.Save();

            return NoContent();
        }

        [HttpPost("{slug}/favorite")]
        public ActionResult<ArticleDto> FavoriteArticle(string slug)
        {
            var Article = _IArticleRepository.GetArticle(slug);
            if (Article == null)
            {
                return NotFound();
            }

            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var CurrentUser = _IUserRepository.GetUser(CurrentUsername);
            

            var ArticleFavorited = _IArticleRepository.FavoriteArticle(CurrentUser, Article);
            if (ArticleFavorited)
            {
                _IArticleRepository.Save();

                var ArticleToReturn = _mapper.Map<ArticleDto>(Article);
                ArticleToReturn.favorited = true;

                return Ok(new { article = ArticleToReturn });
            }

            return BadRequest($"You already favorite the article with slug {slug}");
            //var ff = _IUserRepository.FollowUser(currUser, user);
        }

        [HttpDelete("{slug}/favorite")]
        public IActionResult UnFavoriteArticle(string slug)
        {
            var Article = _IArticleRepository.GetArticle(slug);
            if (Article == null)
            {
                return NotFound();
            }

            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var CurrentUser = _IUserRepository.GetUser(CurrentUsername);

            var ArticleUnfavorited = _IArticleRepository.UnfavoriteArticle(CurrentUser, Article);
            if (ArticleUnfavorited)
            {
                _IArticleRepository.Save();

                var ArticleToReturn = _mapper.Map<ArticleDto>(Article);
                ArticleToReturn.favorited = false;

                return Ok(new { article = ArticleToReturn });
            }
            return BadRequest($" You aren't favorite the article with slug {slug}");
        }

    }
}