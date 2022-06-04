using AutoMapper;
using RealWord.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealWord.Data.Repositories;
using RealWord.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using RealWord.Utils.Utils;
using System.Security.Claims;
using RealWord.Utils.ResourceParameters;
using RealWord.Core.Auth;
using Microsoft.AspNetCore.Http;

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
        private readonly IAuthentication _IAuthentication;
        private readonly ITagRepository _ITagRepository;

        public ArticlesController(IArticleRepository articleRepository, IAuthentication authentication,
               ICommentRepository commentRepository, IUserRepository userRepository,
               ITagRepository tagRepository, IMapper mapper)
        {
            _IArticleRepository = articleRepository ??
                throw new ArgumentNullException(nameof(articleRepository));
            _ITagRepository = tagRepository ??
                throw new ArgumentNullException(nameof(tagRepository));
            _IAuthentication = authentication ??
          throw new ArgumentNullException(nameof(UserRepository));
            _IUserRepository = userRepository ??
            throw new ArgumentNullException(nameof(userRepository));
            _ICommentRepository = commentRepository ??
               throw new ArgumentNullException(nameof(commentRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles([FromQuery] ArticlesParameters articlesParameters)
        {
            var articles = await _IArticleRepository.GetArticlesAsync(articlesParameters);
            if (articles == null)
            {
                return NotFound("Their is no atricles");
            }
            int articlesCount = articles.Count();

            var articlesWhenLogin = new List<ArticleDto>();

            var currentUser = await _IAuthentication.GetCurrentUserAsync();
            if (currentUser != null)
            {
                foreach (var article in articles)
                {
                    var articleDto = _mapper.Map<ArticleDto>(article, a => a.Items["currentUserId"] = currentUser.UserId);
                    var profileDto = _mapper.Map<ProfileDto>(article.User, a => a.Items["currentUserId"] = currentUser.UserId);
                    articleDto.Author = profileDto;
                    articlesWhenLogin.Add(articleDto);
                }

                return Ok(new { articles = articlesWhenLogin, articlesCount = articlesCount });
            }

            var articlesToReturn = _mapper.Map<IEnumerable<ArticleDto>>(articles, a => a.Items["currentUserId"] = Guid.NewGuid());
            return Ok(new { articles = articlesToReturn, articlesCount = articlesCount });
        }

        [HttpGet("feed")]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> FeedArticle([FromQuery] FeedArticlesParameters feedArticlesParameters)
        {
            var currentUser = await _IAuthentication.GetCurrentUserAsync();

            var articles = await _IArticleRepository.GetFeedArticlesAsync(currentUser.UserId, feedArticlesParameters);
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
        public async Task<ActionResult<ArticleDto>> GetArticle(string slug)
        {
            var article = await _IArticleRepository.GetArticleAsync(slug);
            if (article == null)
            {
                return NotFound();
            }

            var articleToReturn = _mapper.Map<ArticleDto>(article, a => a.Items["currentUserId"] = Guid.NewGuid());
            return Ok(new { article = articleToReturn });
        }

        [HttpPost]
        public async Task<ActionResult<ArticleDto>> CreateArticle(ArticleForCreationDto articleForCreation)
        {
            var articleEntityForCreation = _mapper.Map<Article>(articleForCreation);

            articleEntityForCreation.UserId = Guid.NewGuid();
            articleEntityForCreation.Slug.GenerateSlug(articleEntityForCreation.Title, articleEntityForCreation.UserId);

            var currentUser = await _IAuthentication.GetCurrentUserAsync();
            articleEntityForCreation.UserId = currentUser.UserId;

            var timeStamp = DateTime.Now;
            articleEntityForCreation.CreatedAt = timeStamp;
            articleEntityForCreation.UpdatedAt = timeStamp;

            _IArticleRepository.CreateArticle(articleEntityForCreation);
            if (articleForCreation.TagList != null && articleForCreation.TagList.Any())
            {
                _ITagRepository.CreateTags(articleForCreation.TagList, articleEntityForCreation.ArticleId);
            }
            await _IArticleRepository.SaveChangesAsync();
            await _ITagRepository.SaveChangesAsync();

            var articleToReturn = _mapper.Map<ArticleDto>(articleEntityForCreation, a => a.Items["currentUserId"] = currentUser.UserId);
            return new ObjectResult(new { article = articleToReturn }) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpPut("{slug}")]
        public async Task<ActionResult<ArticleDto>> UpdateArticle(string slug, ArticleForUpdateDto articleForUpdate)
        {
            var article = await _IArticleRepository.GetArticleAsync(slug);
            if (article == null)
            {
                return NotFound();
            }

            var currentUser = await _IAuthentication.GetCurrentUserAsync();
            if (currentUser.UserId != article.UserId)
            {
                return BadRequest();
            }

            var articleEntityForUpdate = _mapper.Map<Article>(articleForUpdate);

            if (!string.IsNullOrWhiteSpace(articleForUpdate.Title))
            {
                article.Title = articleForUpdate.Title;
                article.Slug.GenerateSlug(articleForUpdate.Title, article.ArticleId);
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
            await _IArticleRepository.SaveChangesAsync();

            var articleToReturn = _mapper.Map<ArticleDto>(article, a => a.Items["currentUserId"] = currentUser.UserId);
            return Ok(new { article = articleToReturn });
        }

        [HttpDelete("{slug}")]
        public async Task<IActionResult> DeleteArticle(string slug)
        {
            var article = await _IArticleRepository.GetArticleAsync(slug);
            if (article == null)
            {
                return NotFound();
            }

            var currentUser = await _IAuthentication.GetCurrentUserAsync();
            if (currentUser.UserId != article.UserId)
            {
                return BadRequest();
            }

            _IArticleRepository.DeleteArticle(article);
            await _IArticleRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{slug}/favorite")]
        public async Task<ActionResult<ArticleDto>> FavoriteArticle(string slug)
        {
            var article = await _IArticleRepository.GetArticleAsync(slug);
            if (article == null)
            {
                return NotFound();
            }

            var currentUser = await _IAuthentication.GetCurrentUserAsync();

            var isFavorited = await _IArticleRepository.IsFavoritedAsync(currentUser.UserId, article.ArticleId);
            if (isFavorited)
            {
                return BadRequest($"You already favorite the article with slug {slug}");
            }

            _IArticleRepository.FavoriteArticle(currentUser.UserId, article.ArticleId);
            await _IArticleRepository.SaveChangesAsync();

            var articleToReturn = _mapper.Map<ArticleDto>(article, a => a.Items["currentUserId"] = currentUser.UserId);
            return new ObjectResult(new { article = articleToReturn }) { StatusCode = StatusCodes.Status201Created };

        }

        [HttpDelete("{slug}/favorite")]
        public async Task<IActionResult> UnFavoriteArticle(string slug)
        {
            var article = await _IArticleRepository.GetArticleAsync(slug);
            if (article == null)
            {
                return NotFound();
            }

            var currentUser = await _IAuthentication.GetCurrentUserAsync();

            var isFavorited = await _IArticleRepository.IsFavoritedAsync(currentUser.UserId, article.ArticleId);
            if (isFavorited)
            {
                return BadRequest($" You aren't favorite the article with slug {slug}");
            }

            _IArticleRepository.UnfavoriteArticle(currentUser.UserId, article.ArticleId);
            await _IArticleRepository.SaveChangesAsync();

            var articleToReturn = _mapper.Map<ArticleDto>(article, a => a.Items["currentUserId"] = currentUser.UserId);
            return Ok(new { article = articleToReturn });
        }
    }
}