using Microsoft.EntityFrameworkCore;
using RealWord.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly RealWordDbContext _context;

        public CommentRepository(RealWordDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CommentExistsAsync(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            bool commentExists = await _context.Comments.AnyAsync(c => c.CommentId == id);
            return commentExists;
        }
        public async Task<Comment> GetCommentAsync(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var comment = await _context.Comments.Include(c => c.User)
                                                 .FirstOrDefaultAsync(c => c.CommentId == id);
            return comment;
        }
        public async Task<List<Comment>> GetCommentsForArticleAsync(Guid articleId)
        {
            var articleComments = await _context.Comments.Where(c => c.ArticleId == articleId)
                                                   .Include(c => c.User)
                                                   .ThenInclude(c => c.Followers)
                                                   .ToListAsync();
            return articleComments;
        }
        public async Task CreateCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
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
