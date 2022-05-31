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
        public ActionResult<CommentDto> AddCommentToArticle(string slug, CommentForCreationDto CommentForCreation)
        {
            if (!_IArticleRepository.ArticleExists(slug))
            {
                return NotFound();
            }

            var CommentEntityForCreation = _mapper.Map<Comment>(CommentForCreation);

            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var CurrentUser = _IUserRepository.GetUser(CurrentUsername);
            CommentEntityForCreation.UserId = CurrentUser.UserId;

            var Article = _IArticleRepository.GetArticle(slug);
            CommentEntityForCreation.ArticleId = Article.ArticleId;

            var CreatedComment = _ICommentRepository.CreateComment(CommentEntityForCreation);
            _ICommentRepository.Save();

            var CreatedCommentToReturn = _mapper.Map<CommentDto>(CreatedComment);
            return Ok(new { comment = CreatedCommentToReturn });
        }

        [AllowAnonymous]
        [HttpGet("{slug}/comments")]
        public ActionResult<IEnumerable<CommentDto>> GetCommentsFromArticle(string slug)
        {
            if (!_IArticleRepository.ArticleExists(slug))
            {
                return NotFound();
            }

            var Comments = _ICommentRepository.GetAllComments(slug);
            var CommentsToReturn = _mapper.Map<IEnumerable<CommentDto>>(Comments);

            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (CurrentUsername != null)
            {
                //أفحص اذا هاي الكومنتات اله وشو ارجع الفلو
                foreach (var x in CommentsToReturn)
                {//هون بدي اعمل موضوع الفلو لما يكون مسجل دخول 
                    x.author.following = true;
                    //أعمل كود برجعلي برفايل مع انه متابعه أو لا
                }
            }

            return Ok(new { comments = CommentsToReturn });
        }

        [HttpDelete("{slug}/comments/{id}")]
        public IActionResult DeleteComment(string slug, Guid id)
        {
            if (!_IArticleRepository.ArticleExists(slug))
            {
                return NotFound();
            }

            var Comment = _ICommentRepository.GetComment(id);
            if (Comment == null)
            {
                return NotFound();
            }

            _ICommentRepository.DeleteComment(Comment);
            _ICommentRepository.Save();

            return NoContent();
        }
    }
}
