using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealWord.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealWord.Core.Services;

namespace RealWord.Web.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/articles")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _ICommentService;

        public CommentsController(ICommentService commentService)
        {
            _ICommentService = commentService ??
                throw new ArgumentNullException(nameof(commentService));
        }

        [HttpPost("{slug}/comments")]
        public async Task<ActionResult<CommentDto>> AddCommentToArticle(string slug, CommentForCreationDto commentForCreation)
        {
            var createdCommentToReturn = await _ICommentService.AddCommentToArticleAsync(slug, commentForCreation);
            if (createdCommentToReturn == null)
            {
                return NotFound();
            }

            return new ObjectResult(new { comment = createdCommentToReturn }) { StatusCode = StatusCodes.Status201Created };
        }

        [AllowAnonymous]
        [HttpGet("{slug}/comments")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsFromArticle(string slug)
        {
            var commentsToReturn = await _ICommentService.GetCommentsFromArticleAsync(slug);
            if (commentsToReturn == null)
            {
                return NotFound();
            }

            return Ok(new { comments = commentsToReturn });
        }

        [HttpDelete("{slug}/comments/{id}")]
        public async Task<IActionResult> DeleteComment(string slug, Guid id)
        {
            var isArticleExists = await _ICommentService.CommentExistsAsync(slug, id);
            if (!isArticleExists)
            {
                return NotFound();
            }

            var isAuthorized = await _ICommentService.IsAuthorized(slug, id);
            if (!isAuthorized)
            {
                return Forbid();
            }

            await _ICommentService.DeleteCommentAsync(id);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpOptions("{slug}/comments")]
        public IActionResult CommentsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,Delete");

            return Ok();
        }
    }
}