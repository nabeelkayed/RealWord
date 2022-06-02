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
        public ActionResult<IEnumerable<ArticleDto>> GetArticles([FromQuery] ArticlesParameters articlesParameters)
        {
            var articles = _IArticleRepository.GetArticles(articlesParameters);
            if (articles == null)
            {
                return NotFound("Their is no atricles");
            }
            int articlesCount = articles.Count();

            var currentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUsername != null)
            {
                var currentUserId = _IUserRepository.GetUser(currentUsername);
                var articlesWhenLogin = new List<ArticleDto>();

                foreach (var article in articles)
                {
                    var articleDto = _mapper.Map<ArticleDto>(article, a => a.Items["currentUserId"] = currentUserId);
                    var profileDto = _mapper.Map<ProfileDto>(article.User, a => a.Items["currentUserId"] = currentUserId);
                    articleDto.Author = profileDto;
                    articlesWhenLogin.Add(articleDto);
                }

                return Ok(new { articles = articlesWhenLogin, articlesCount = articlesCount });
            }

            var articlesToReturn = _mapper.Map<IEnumerable<ArticleDto>>(articles, a => a.Items["currentUserId"] = Guid.NewGuid());

            return Ok(new { articles = articlesToReturn, articlesCount = articlesCount });
        }

        [HttpGet("feed")]
        public ActionResult<IEnumerable<ArticleDto>> FeedArticle([FromQuery] FeedArticlesParameters feedArticlesParameters)
        {
            var currentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = _IUserRepository.GetUser(currentUsername);

            var articles = _IArticleRepository.GetFeedArticles(currentUser.UserId, feedArticlesParameters);
            if (!articles.Any())
            {
                return NotFound("The followings have no articles");
            }
            int articlesCount = articles.Count();

            var articlesToReturn = new List<ArticleDto>();
            foreach (var article in articles)
            {
                var profileDto = _mapper.Map<ProfileDto>(article.User, a => a.Items["currentUserId"] = currentUser.UserId);
                var articleDto = _mapper.Map<ArticleDto>(article, a => a.Items["currentUserId"] = currentUser.UserId);
                articleDto.Author = profileDto;
                articlesToReturn.Add(articleDto);
            }

            return Ok(new { articles = articlesToReturn, articlesCount = articlesCount });
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

            var articleToReturn = _mapper.Map<ArticleDto>(article, a => a.Items["currentUserId"] = Guid.NewGuid());
            return Ok(new { article = articleToReturn });
        }

        [HttpPost]
        public ActionResult<ArticleDto> CreateArticle(ArticleForCreationDto articleForCreation)
        {
            var articleEntityForCreation = _mapper.Map<Article>(articleForCreation);
            articleEntityForCreation.Slug = Slug.GenerateSlug(articleEntityForCreation.Title);

            var currentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var currentUserId = _IUserRepository.GetUser(currentUsername).UserId;
            articleEntityForCreation.UserId = currentUserId;

            var timeStamp = DateTime.Now;
            articleEntityForCreation.CreatedAt = timeStamp;
            articleEntityForCreation.UpdatedAt = timeStamp;

            _IArticleRepository.CreateArticle(articleEntityForCreation, articleForCreation.TagList);
            _IArticleRepository.Save();

            var ArticleToReturn = _mapper.Map<ArticleDto>(articleEntityForCreation, a => a.Items["currentUserId"] = currentUserId);
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
            var currentUserId = _IUserRepository.GetUser(currentUsername).UserId;

            if (currentUserId != article.UserId)
            {
                return BadRequest();
            }

            var articleEntityForUpdate = _mapper.Map<Article>(articleForUpdate);

            if (!string.IsNullOrWhiteSpace(articleForUpdate.Title))
            {
                article.Title = articleForUpdate.Title;
                article.Slug = Slug.GenerateSlug(articleForUpdate.Title);
            }
            if (!string.IsNullOrWhiteSpace(articleForUpdate.Description))
            {
                article.Description = articleForUpdate.Description;
            }
            if (!string.IsNullOrWhiteSpace(articleForUpdate.Body))
            {
                article.Body = articleForUpdate.Body;
            }

            article.UpdatedAt = DateTime.Now;

            _IArticleRepository.UpdateArticle(article, articleEntityForUpdate);
            _IArticleRepository.Save();

            var articleToReturn = _mapper.Map<ArticleDto>(article, a => a.Items["currentUserId"] = currentUserId);
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

            var currentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var currentUserId = _IUserRepository.GetUser(currentUsername).UserId;
            if (currentUserId != article.UserId)
            {
                return BadRequest();
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
            var currentUserId = _IUserRepository.GetUser(currentUsername).UserId;

            var isFavorited = _IArticleRepository.IsFavorited(currentUserId, article.ArticleId);
            if (isFavorited)
            {
                return BadRequest($"You already favorite the article with slug {slug}");
            }

            _IArticleRepository.FavoriteArticle(currentUserId, article.ArticleId);
            _IArticleRepository.Save();

            var articleToReturn = _mapper.Map<ArticleDto>(article, a => a.Items["currentUserId"] = currentUserId);
            return Ok(new { article = articleToReturn });
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
            var currentUserId = _IUserRepository.GetUser(currentUsername).UserId;

            var isUnfavorited = !_IArticleRepository.IsFavorited(currentUserId, article.ArticleId);

            if (isUnfavorited)
            {
                return BadRequest($" You aren't favorite the article with slug {slug}");
            }
            _IArticleRepository.UnfavoriteArticle(currentUserId, article.ArticleId);
            _IArticleRepository.Save();

            var articleToReturn = _mapper.Map<ArticleDto>(article, a => a.Items["currentUserId"] = currentUserId);
            return Ok(new { article = articleToReturn });
        }
    }
}