using Microsoft.EntityFrameworkCore;
using RealWord.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealWord.Db.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly RealWordDbContext _context;

        public CommentRepository(RealWordDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public Comment GetComment(Guid id)
        {
            return _context.Comments.Find(id);
        }
        public List<Comment> GetAllComments(string Slug)
        {
            var Article = _context.Articles.FirstOrDefault(a => a.Slug == Slug);
            var Comments = _context.Comments.Where(c => c.ArticleId == Article.ArticleId).ToList();
            
            return Comments;
        }
        public Comment CreateComment(Comment Comment)
        {
            Comment.CommentId = Guid.NewGuid();

            var TimeStamp = DateTime.Now;
            Comment.CreatedAt = TimeStamp;
            Comment.UpdatedAt = TimeStamp;

            _context.Comments.Add(Comment);


           var xx = _context.Comments.Find(Comment.CommentId);
          /*  var xx = _context.Comments.Where(c => c.CommentId == comment.CommentId)
                .Select(c=>new {c.Article,c.ArticleId,c.Body,c.CommentId,c.User,c.UserId, createdAt =EF.Property<DateTime>(c, "CreatedAt") }).ToList();*/
            return xx;
        }
        public void DeleteComment(Comment comment)
        {
            _context.Comments.Remove(comment);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
