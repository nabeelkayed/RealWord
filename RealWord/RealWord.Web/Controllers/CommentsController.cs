using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealWord.Db.Entities;
using RealWord.Db.Repositories;
using RealWord.Web.Helpers;
using RealWord.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


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

        public CommentsController(IArticleRepository ArticleRepository, IUserRepository userRepository,
        IAuthentication authentication, ICommentRepository CommentRepository,
        IMapper mapper)
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
        }

        [HttpPost("{slug}/comments")]
        public ActionResult<CommentDto> AddCommentToArticle(string slug, CommentForCreationDto commentForCreation)
        {
            var ArticleNotExists = !_IArticleRepository.ArticleExists(slug);
            if (ArticleNotExists)
            {
                return NotFound();
            }

            var commentEntityForCreation = _mapper.Map<Comment>(commentForCreation);

            var currentUser = _IAuthentication.GetCurrentUser();
            commentEntityForCreation.UserId = currentUser.UserId;

            var Article = _IArticleRepository.GetArticle(slug);
            commentEntityForCreation.ArticleId = Article.ArticleId;

            var timeStamp = DateTime.Now;
            commentEntityForCreation.CreatedAt = timeStamp;
            commentEntityForCreation.UpdatedAt = timeStamp;

            _ICommentRepository.CreateComment(commentEntityForCreation);
            _ICommentRepository.Save();

            var createdCommentToReturn = _mapper.Map<CommentDto>(commentEntityForCreation);
            return new ObjectResult(new { comment = createdCommentToReturn }) { StatusCode = StatusCodes.Status201Created };
        }

        [AllowAnonymous]
        [HttpGet("{slug}/comments")]
        public ActionResult<IEnumerable<CommentDto>> GetCommentsFromArticle(string slug)
        {
            var article = _IArticleRepository.GetArticle(slug);

            if (article == null)
            {
                return NotFound();
            }

            var comments = _ICommentRepository.GetCommentsForArticle(article.ArticleId);
            var commentsWhenLogin = new List<CommentDto>();

            var currentUser = _IAuthentication.GetCurrentUser();
            if (currentUser.Username != null)
            {
                foreach (var comment in comments)
                {
                    var commentDto = _mapper.Map<CommentDto>(comment, a => a.Items["currentUserId"] = currentUser.UserId);
                    var profileDto = _mapper.Map<ProfileDto>(comment.User, a => a.Items["currentUserId"] = currentUser.UserId);
                    commentDto.Author = profileDto;
                    commentsWhenLogin.Add(commentDto);
                }

                return Ok(new { comments = commentsWhenLogin });
            }

            var commentsToReturn1 = _mapper.Map<IEnumerable<CommentDto>>(comments);
            return Ok(new { comments = commentsToReturn1 });
        }

        [HttpDelete("{slug}/comments/{id}")]
        public IActionResult DeleteComment(string slug, Guid id)
        {
            if (!_IArticleRepository.ArticleExists(slug))
            {
                return NotFound();
            }

            var comment = _ICommentRepository.GetComment(id);
            if (comment == null)
            {
                return NotFound();
            }

            var currentUser = _IAuthentication.GetCurrentUser();
            if (currentUser.UserId != comment.UserId)
            {
                return BadRequest();
            }

            _ICommentRepository.DeleteComment(comment);
            _ICommentRepository.Save();

            return NoContent();
        }
    }
}