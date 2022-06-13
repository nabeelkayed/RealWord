using RealWord.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealWord.Core.Services
{
    public interface ICommentService
    {
        Task<bool> CommentExistsAsync(string slug, Guid id);
        Task<bool> IsAuthorized(string slug, Guid id);
        Task<CommentDto> AddCommentToArticleAsync(string slug, CommentForCreationDto commentForCreation);
        Task<IEnumerable<CommentDto>> GetCommentsFromArticleAsync(string slug);
        Task DeleteCommentAsync(Guid id);
    }
}