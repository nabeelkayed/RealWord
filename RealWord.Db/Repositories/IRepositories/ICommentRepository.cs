using RealWord.Db.Entities;
using System;
using System.Collections.Generic;

namespace RealWord.Db.Repositories
{
    public interface ICommentRepository
    {
        void CreateComment(Comment Comment);
        List<Comment> GetCommentsForArticle(string Slug);
        void DeleteComment(Comment Comment);
        Comment GetComment(Guid Id);
        void Save(); 
    }
}