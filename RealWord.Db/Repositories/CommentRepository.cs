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
        public Comment GetComment(Guid id)//id مش لازم Guid
        {
            if (id == null || id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var comment = _context.Comments.Find(id);
            return comment;
        }
        public List<Comment> GetCommentsForArticle(string slug)
        {
            var article = _context.Articles.FirstOrDefault(a => a.Slug == slug);
            var articleComments = _context.Comments.Where(c => c.ArticleId == article.ArticleId).ToList();

            return articleComments;
        }
        public void CreateComment(Comment comment)
        {
            _context.Comments.Add(comment);
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
