using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealWord.Data.Entities;
using RealWord.Data.Repositories;
using RealWord.Core.Auth;
using RealWord.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using RealWord.Core.Repositories;

namespace RealWord.Web.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/articles")]
    public class CommentsController : ControllerBase
    {
        private readonly IArticleRepository _IArticleRepository;
        private readonly ICommentRepository _ICommentRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _IUserRepository;
        private readonly IAuthentication _IAuthentication;
        private readonly IUserService _IUserService;


        public CommentsController(IArticleRepository ArticleRepository, IUserRepository userRepository,
        IAuthentication authentication, ICommentRepository CommentRepository,
        IMapper mapper, IUserService userService)
        {
            _IArticleRepository = ArticleRepository ??
                throw new ArgumentNullException(nameof(ArticleRepository));
            _IAuthentication = authentication ??
          throw new ArgumentNullException(nameof(UserRepository));
            _IUserRepository = userRepository ??
           throw new ArgumentNullException(nameof(userRepository));
            _ICommentRepository = CommentRepository ??
               throw new ArgumentNullException(nameof(CommentRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _IUserService = userService ??
             throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost("{slug}/comments")]
        public async Task<ActionResult<CommentDto>> AddCommentToArticle(string slug, CommentForCreationDto commentForCreation)
        {
            var ArticleExists = await _IArticleRepository.ArticleExistsAsync(slug);
            if (!ArticleExists)
            {
                return NotFound();
            }

            var commentEntityForCreation = _mapper.Map<Comment>(commentForCreation);

            var currentUserId = await _IUserService.GetCurrentUserIdAsync();
            commentEntityForCreation.UserId = currentUserId;

            var Article = await _IArticleRepository.GetArticleAsync(slug);
            commentEntityForCreation.ArticleId = Article.ArticleId;

            var timeStamp = DateTime.Now;
            commentEntityForCreation.CreatedAt = timeStamp;
            commentEntityForCreation.UpdatedAt = timeStamp;

            _ICommentRepository.CreateComment(commentEntityForCreation);
            await _ICommentRepository.SaveChangesAsync();

            var createdCommentToReturn = _mapper.Map<CommentDto>(commentEntityForCreation);
            return new ObjectResult(new { comment = createdCommentToReturn }) { StatusCode = StatusCodes.Status201Created };
        }

        [AllowAnonymous]
        [HttpGet("{slug}/comments")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsFromArticle(string slug)
        {
            var article = await _IArticleRepository.GetArticleAsync(slug);

            if (article == null)
            {
                return NotFound();
            }

            var comments = await _ICommentRepository.GetCommentsForArticleAsync(article.ArticleId);
            var commentsWhenLogin = new List<CommentDto>();

            var currentUserId = await _IUserService.GetCurrentUserIdAsync();
            if (currentUserId != null)
            {
                foreach (var comment in comments)
                {
                    var commentDto = _mapper.Map<CommentDto>(comment, a => a.Items["currentUserId"] = currentUserId);
                    var profileDto = _mapper.Map<ProfileDto>(comment.User, a => a.Items["currentUserId"] = currentUserId);
                    commentDto.Author = profileDto;
                    commentsWhenLogin.Add(commentDto);
                }

                return Ok(new { comments = commentsWhenLogin });
            }

            var commentsToReturn1 = _mapper.Map<IEnumerable<CommentDto>>(comments);
            return Ok(new { comments = commentsToReturn1 });
        }

        [HttpDelete("{slug}/comments/{id}")]
        public async Task<IActionResult> DeleteComment(string slug, Guid id)
        {
            var articleExists = await _IArticleRepository.ArticleExistsAsync(slug);
            if (!articleExists)
            {
                return NotFound();
            }

            var comment = await _ICommentRepository.GetCommentAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var currentUserId = await _IUserService.GetCurrentUserIdAsync();
            if (currentUserId != comment.UserId)
            {
                return BadRequest();
            }

            _ICommentRepository.DeleteComment(comment);
            await _ICommentRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}