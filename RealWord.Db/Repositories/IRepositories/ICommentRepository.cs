using RealWord.Db.Entities;
using System;
using System.Collections.Generic;

namespace RealWord.Db.Repositories
{
    public interface ICommentRepository
    {
        Comment GetComment(Guid Id);
        List<Comment> GetAllComments(string Slug);
        Comment CreateComment(Comment Comment); 
        void DeleteComment(Comment Comment);
        void Save(); 
    }
}