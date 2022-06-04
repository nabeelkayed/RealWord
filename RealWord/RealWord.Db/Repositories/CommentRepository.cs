﻿using Microsoft.EntityFrameworkCore;
using RealWord.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWord.Db.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly RealWordDbContext _context;

        public CommentRepository(RealWordDbContext context)
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
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
