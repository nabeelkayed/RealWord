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
        public void CreateComment(Comment Comment)
        {
            _context.Comments.Add(Comment);
        }
        public List<Comment> GetCommentsForArticle(string Slug)
        {
            var Article = _context.Articles.FirstOrDefault(a => a.Slug == Slug);
            var Comments = _context.Comments.Where(c => c.ArticleId == Article.ArticleId).ToList();

            return Comments;
        }
        public Comment GetComment(Guid Id)//id مش لازم Guid
        {
            if (Id == null || Id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(Id));
            }

            return _context.Comments.Find(Id);
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
