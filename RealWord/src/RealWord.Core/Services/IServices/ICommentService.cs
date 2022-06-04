using RealWord.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealWord.Core.Repositories
{
    public interface ICommentService
    {
        Task<Comment> GetCommentAsync(Guid id);
        Task<List<Comment>> GetCommentsForArticleAsync(Guid articleId);
        void CreateComment(Comment comment);
        void DeleteComment(Comment comment);
        Task SaveChangesAsync();
    }
}