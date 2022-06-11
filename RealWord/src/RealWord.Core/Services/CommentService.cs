using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealWord.Core.Models;
using RealWord.Data;
using RealWord.Data.Entities;
using RealWord.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWord.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly IArticleRepository _IArticleRepository;//لازم تنشال
        private readonly ICommentRepository _ICommentRepository;
        private readonly IUserService _IUserService;
        private readonly IMapper _mapper;
        private readonly IArticleService _IArticleService;

        public CommentService(IArticleRepository ArticleRepository, ICommentRepository CommentRepository,
        IMapper mapper, IArticleService articleService, IUserService userService)
        {
            _IArticleRepository = ArticleRepository ??
                throw new ArgumentNullException(nameof(ArticleRepository));
            _IArticleService = articleService ??
               throw new ArgumentNullException(nameof(articleService));
            _ICommentRepository = CommentRepository ??
                throw new ArgumentNullException(nameof(CommentRepository));
            _IUserService = userService ??
                throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> CommentExistsAsync(string slug, Guid id)
        {
            var articleExists = await _IArticleRepository.ArticleExistsAsync(slug);
            if (!articleExists)
            {
                return false;
            }

            var comment = await _ICommentRepository.GetCommentAsync(id);
            if (comment == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> IsAuthorized(string slug,Guid id)
        {
            var comment = await _ICommentRepository.GetCommentAsync(id);
            var currentUserId = await _IUserService.GetCurrentUserIdAsync();

            var isAuthorized = currentUserId == comment.UserId;
            return isAuthorized;
        }
        public async Task<CommentDto> AddCommentToArticleAsync(string slug, CommentForCreationDto commentForCreation)
        {
            var ArticleExists = await _IArticleRepository.ArticleExistsAsync(slug);
            if (!ArticleExists)
            {
                return null;
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
            return createdCommentToReturn;
        }
        public async Task<IEnumerable<CommentDto>> GetCommentsFromArticleAsync(string slug)
        {
            var article = await _IArticleRepository.GetArticleAsync(slug);
            if (article == null)
            {
                return null;
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

                return commentsWhenLogin;
            }

            var commentsToReturn = _mapper.Map<IEnumerable<CommentDto>>(comments);
            return commentsToReturn;
        }
        public async Task<bool> DeleteCommentAsync(string slug, Guid id)
        {
            var comment = await _ICommentRepository.GetCommentAsync(id);

            _ICommentRepository.DeleteComment(comment);
            await _ICommentRepository.SaveChangesAsync();
            return true;
        }
    }
}
