using AutoMapper;
using RealWord.Core.Models;
using RealWord.Data.Entities;
using RealWord.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealWord.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _ICommentRepository;
        private readonly IArticleService _IArticleService;
        private readonly IUserService _IUserService;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepository CommentRepository,
        IArticleService articleService, IUserService userService, IMapper mapper)
        {
            _ICommentRepository = CommentRepository ??
               throw new ArgumentNullException(nameof(CommentRepository));
            _IArticleService = articleService ??
               throw new ArgumentNullException(nameof(articleService));
            _IUserService = userService ??
                throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> CommentExistsAsync(string slug, Guid id)
        {
            var articleExists = await _IArticleService.ArticleExistsAsync(slug);
            if (!articleExists)
            {
                return false;
            }

            var commentExists = await _ICommentRepository.CommentExistsAsync(id);
            if (!commentExists)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> IsAuthorized(string slug, Guid id)
        {
            var comment = await _ICommentRepository.GetCommentAsync(id);
            var currentUserId = await _IUserService.GetCurrentUserIdAsync();

            var isAuthorized = currentUserId == comment.UserId;
            return isAuthorized;
        }
        public async Task<CommentDto> AddCommentToArticleAsync(string slug, CommentForCreationDto commentForCreation)
        {
            var articleId = await _IArticleService.GetArticleIdAsync(slug);
            if (articleId == Guid.Empty)
            {
                return null;
            }

            var commentEntityForCreation = _mapper.Map<Comment>(commentForCreation);

            commentEntityForCreation.CommentId = Guid.NewGuid();

            var currentUserId = await _IUserService.GetCurrentUserIdAsync();
            commentEntityForCreation.UserId = currentUserId;

            commentEntityForCreation.ArticleId = articleId;

            var timeStamp = DateTime.Now;
            commentEntityForCreation.CreatedAt = timeStamp;
            commentEntityForCreation.UpdatedAt = timeStamp;

            await _ICommentRepository.CreateCommentAsync(commentEntityForCreation);
            await _ICommentRepository.SaveChangesAsync();

            var createdCommentToreturn = MapComment(currentUserId, commentEntityForCreation);
            return createdCommentToreturn;
        }
        public async Task<IEnumerable<CommentDto>> GetCommentsFromArticleAsync(string slug)
        {
            var articleId = await _IArticleService.GetArticleIdAsync(slug);
            if (articleId == Guid.Empty)
            {
                return null;
            }

            var comments = await _ICommentRepository.GetCommentsForArticleAsync(articleId);

            var commentsToReturn = new List<CommentDto>();
            var currentUserId = await _IUserService.GetCurrentUserIdAsync();

            foreach (var comment in comments)
            {
                var commentDto = MapComment(currentUserId, comment);
                commentsToReturn.Add(commentDto);
            }

            return commentsToReturn;
        }

        public async Task DeleteCommentAsync(Guid id)
        {
            var comment = await _ICommentRepository.GetCommentAsync(id);

            _ICommentRepository.DeleteComment(comment);
            await _ICommentRepository.SaveChangesAsync();          
        }
        private CommentDto MapComment(Guid currentUserId, Comment comment)
        {
            var commentDto = _mapper.Map<CommentDto>(comment);
            var profileDto = _mapper.Map<ProfileDto>(comment.User, a => a.Items["currentUserId"] = currentUserId);
            commentDto.Author = profileDto;
            
            return commentDto;
        }
    }
}
