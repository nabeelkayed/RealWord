using RealWord.Db.Entities;
using System;
using System.Collections.Generic;

namespace RealWord.Db.Repositories
{
    public interface ICommentRepository
    {
        Comment GetComment(Guid id);
        List<Comment> GetCommentsForArticle(Guid articleId);
        void CreateComment(Comment comment);
        void DeleteComment(Comment comment);
        void Save();
    }
}