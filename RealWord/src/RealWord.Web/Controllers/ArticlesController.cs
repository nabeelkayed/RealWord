using RealWord.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RealWord.Utils.ResourceParameters;
using Microsoft.AspNetCore.Http;
using RealWord.Core.Services;

namespace RealWord.Web.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _IArticleService;

        public ArticlesController(IArticleService articleService)
        {
            _IArticleService = articleService ??
                throw new ArgumentNullException(nameof(articleService));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles([FromQuery] ArticlesParameters articlesParameters)
        {
            var articlesToReturn = await _IArticleService.GetArticlesAsync(articlesParameters);
            if (articlesToReturn == null)
            {
                return NotFound();
            }

            return Ok(new { articles = articlesToReturn, articlesCount = articlesToReturn.Count() });
        }

        [HttpGet("feed")]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetFeedArticle([FromQuery] FeedArticlesParameters feedArticlesParameters)
        {
            var articlesToReturn = await _IArticleService.FeedArticleAsync(feedArticlesParameters);
            if (articlesToReturn == null)
            {
                return NotFound();
            }

            return Ok(new { articles = articlesToReturn, articlesCount = articlesToReturn.Count() });
        }

        [AllowAnonymous]
        [HttpGet("{slug}")]
        public async Task<ActionResult<ArticleDto>> GetArticle(string slug)
        {
            var articleToReturn = await _IArticleService.GetArticleAsync(slug);
            if (articleToReturn == null)
            {
                return NotFound();
            }

            return Ok(new { article = articleToReturn });
        }

        [HttpPost]
        public async Task<ActionResult<ArticleDto>> CreateArticle(ArticleForCreationDto articleForCreation)
        {
            var CreatedArticleToReturn = await _IArticleService.CreateArticleAsync(articleForCreation);

            return new ObjectResult(new { article = CreatedArticleToReturn }) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpPut("{slug}")]
        public async Task<ActionResult<ArticleDto>> UpdateArticle(string slug, ArticleForUpdateDto articleForUpdate)
        {
            var isArticleExists = await _IArticleService.ArticleExistsAsync(slug);
            if (!isArticleExists)
            {
                return NotFound();
            }

            var isAuthorized = await _IArticleService.IsAuthorized(slug);
            if (!isAuthorized)
            {
                return Forbid();
            }

            var ubdatedArticleToReturn = await _IArticleService.UpdateArticleAsync(slug, articleForUpdate);
            return Ok(new { article = ubdatedArticleToReturn });
        }

        [HttpDelete("{slug}")]
        public async Task<IActionResult> DeleteArticle(string slug)
        {
            var isArticleExists = await _IArticleService.ArticleExistsAsync(slug);
            if (!isArticleExists)
            {
                return NotFound();
            }

            var isAuthorized = await _IArticleService.IsAuthorized(slug);
            if (!isAuthorized)
            {
                return Forbid();
            }

            await _IArticleService.DeleteArticleAsync(slug);
            return NoContent();
        }

        [HttpPost("{slug}/favorite")]
        public async Task<ActionResult<ArticleDto>> FavoriteArticle(string slug)
        {
            var FavoritedaArticleToReturn = await _IArticleService.FavoriteArticleAsync(slug);
            if (FavoritedaArticleToReturn == null)
            {
                return NotFound();
            }

            return new ObjectResult(new { article = FavoritedaArticleToReturn }) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpDelete("{slug}/favorite")]
        public async Task<ActionResult<ArticleDto>> UnFavoriteArticle(string slug)
        {
            var unfavoritedaArticleToReturn = await _IArticleService.UnFavoriteArticleAsync(slug);
            if (unfavoritedaArticleToReturn == null)
            {
                return NotFound();
            }

            return Ok(new { article = unfavoritedaArticleToReturn });
        }

        [AllowAnonymous]
        [HttpOptions("feed")]
        public IActionResult ArticlesFeedOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS");

            return Ok();
        }

        [AllowAnonymous]
        [HttpOptions("{slug}/favorite")]
        public IActionResult ArticlesFavoriteOptions()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST,Delete");

            return Ok();
        }
        [AllowAnonymous]
        [HttpOptions]
        public IActionResult ArticlesOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,Delete,PUT");

            return Ok();
        }
    }
}