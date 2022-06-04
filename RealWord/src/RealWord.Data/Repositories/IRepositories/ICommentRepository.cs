using RealWord.Db.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealWord.Db.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment> GetCommentAsync(Guid id);
        Task<List<Comment>> GetCommentsForArticleAsync(Guid articleId);
        void CreateComment(Comment comment);
        void DeleteComment(Comment comment);
        void SaveChanges();
    }
}