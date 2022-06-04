using Microsoft.EntityFrameworkCore;
using RealWord.Data;
using RealWord.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWord.Core.Repositories
{
    public class CommentService : ICommentService
    {
        private readonly RealWordDbContext _context;

        public CommentService(RealWordDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Comment> GetCommentAsync(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id)); 
            }

            var comment = await _context.Comments.FindAsync(id);
            return comment;
        }
        public async Task<List<Comment>> GetCommentsForArticleAsync(Guid articleId)
        {
            var articleComments = await _context.Comments.Where(c => c.ArticleId == articleId)
                                                   .Include(c=>c.User)
                                                   .ThenInclude(c=>c.Followers)
                                                   .ToListAsync();
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
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
