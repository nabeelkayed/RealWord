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
        public ActionResult<CommentDto> AddCommentToArticle(string slug, CommentForCreationDto comment)
        {
            if (!_IArticleRepository.ArticleExists(slug))
            {
                return NotFound();
            }

            var commentEntity = _mapper.Map<Comment>(comment);

            var username = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var CurrentUser = _IUserRepository.GetUser(username);

            commentEntity.UserId = CurrentUser.UserId;

            var article = _IArticleRepository.GetArticle(slug);

            commentEntity.ArticleId = article.ArticleId;

            var newcimment = _ICommentRepository.CreateComment(commentEntity);
            _ICommentRepository.Save();

            var c = _mapper.Map<CommentDto>(newcimment);
            return Ok(new { comment = c });

        }

        [AllowAnonymous]
        [HttpGet("{slug}/comments")]
        public ActionResult<IEnumerable<CommentDto>> GetCommentsFromArticle(string slug)
        {
            if (!_IArticleRepository.ArticleExists(slug))
            {
                return NotFound();
            }
            var AllComments = _ICommentRepository.GetAllComments(slug);
            var xx = _mapper.Map<IEnumerable<CommentDto>>(AllComments);

            var currUsername1 = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (currUsername1 == null)
            {
                return Ok(new { comments = xx });

            }
            //أفحص اذا هاي الكومنتات اله وشو ارجع الفلو
            foreach (var x in xx)
            {//هون بدي اعمل موضوع الفلو لما يكون مسجل دخول 
                x.author.following = true;
                //أعمل كود برجعلي برفايل مع انه متابعه أو لا
            }
            return Ok(new { comments = xx });
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
