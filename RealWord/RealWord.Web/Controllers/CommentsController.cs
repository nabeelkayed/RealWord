using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWord.Db.Entities;
using RealWord.Db.Repositories;
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

        public CommentsController(IArticleRepository ArticleRepository, IUserRepository userRepository,
            ICommentRepository CommentRepository,
            IMapper mapper)
        {
            _IArticleRepository = ArticleRepository ??
                throw new ArgumentNullException(nameof(ArticleRepository));
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
            if (!_IArticleRepository.ArticleExists(slug))
            {
                return NotFound();
            }

            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var CurrentUser = _IUserRepository.GetUser(CurrentUsername);

            var commentEntityForCreation = _mapper.Map<Comment>(commentForCreation);

            commentEntityForCreation.UserId = CurrentUser.UserId;

            var Article = _IArticleRepository.GetArticle(slug);
            commentEntityForCreation.ArticleId = Article.ArticleId;

            var timeStamp = DateTime.Now;
            commentEntityForCreation.CreatedAt = timeStamp;
            commentEntityForCreation.UpdatedAt = timeStamp;

            //commentEntityForCreation.CommentId = Guid.NewGuid();

            _ICommentRepository.CreateComment(commentEntityForCreation);
            _ICommentRepository.Save();

            var createdCommentToReturn = _mapper.Map<CommentDto>(commentEntityForCreation);

            return Ok(new { comment = createdCommentToReturn });
        }

        [AllowAnonymous]
        [HttpGet("{slug}/comments")]
        public ActionResult<IEnumerable<CommentDto>> GetCommentsFromArticle(string slug)
        {
            if (!_IArticleRepository.ArticleExists(slug))
            {
                return NotFound();
            }

            var comments = _ICommentRepository.GetCommentsForArticle(slug);
            //var commentsToReturn = _mapper.Map<IEnumerable<CommentDto>>(comments,a => a.Items["currentUserId"] = Guid.NewGuid());

            var currentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUsername != null)
            {
                var currentUser = _IUserRepository.GetUser(currentUsername);
                var commentsToReturn = new List<CommentDto>();

               

                foreach (var comment in comments)
                {
                    var commentDto = _mapper.Map<CommentDto>(comment, a => a.Items["currentUserId"] = currentUser.UserId);
                    var profileDto = _mapper.Map<ProfileDto>(comment.User, a => a.Items["currentUserId"] = currentUser.UserId);
                    commentDto.Author = profileDto;
                    commentsToReturn.Add(commentDto);
                }

                return Ok(new { comments = commentsToReturn });
            }
            var Final = _mapper.Map<IEnumerable<CommentDto>>(comments/*, a => a.Items["currentUserId"] = Guid.NewGuid()*/);
            return Ok(new { comments = Final });
            /* //أفحص اذا هاي الكومنتات اله وشو ارجع الفلو
                 foreach (var comment in commentsToReturn)
                 {//هون بدي اعمل موضوع الفلو لما يكون مسجل دخول 
                     comment.Author.Following = true;
                     //أعمل كود برجعلي برفايل مع انه متابعه أو لا
                 }*/
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

            _ICommentRepository.DeleteComment(comment);
            _ICommentRepository.Save();

            return NoContent();
        }
    }
}
