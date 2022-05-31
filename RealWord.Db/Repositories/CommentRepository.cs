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
        public List<Comment> GetAllComments(string slug)
        {
            var arr = _context.Articles.FirstOrDefault(a => a.Slug == slug);
            var com = _context.Comments.Where(c => c.ArticleId == arr.ArticleId).ToList();
            return com;
        }
        public Comment CreateComment(Comment comment)
        {
            comment.CommentId = Guid.NewGuid();

            var timestamp = DateTime.Now;
            comment.CreatedAt = timestamp;
            comment.UpdatedAt = timestamp;

            _context.Comments.Add(comment);

            /*var timestamp = DateTime.Now;
            _context.Entry(comment).Property("CreatedAt").CurrentValue = timestamp;
            _context.Entry(comment).Property("UpdatedAt").CurrentValue = timestamp;*/

           var xx = _context.Comments.Find(comment.CommentId);
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
