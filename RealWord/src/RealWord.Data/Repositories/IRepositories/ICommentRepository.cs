using RealWord.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealWord.Data.Repositories
{
    public interface ICommentRepository
    {
        Task<bool> CommentExistsAsync(Guid id);
        Task<Comment> GetCommentAsync(Guid id);
        Task<List<Comment>> GetCommentsForArticleAsync(Guid articleId);
        Task CreateCommentAsync(Comment comment);
        void DeleteComment(Comment comment);
        Task SaveChangesAsync();
    }
}