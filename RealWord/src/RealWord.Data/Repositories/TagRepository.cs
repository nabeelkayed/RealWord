using Microsoft.EntityFrameworkCore;
using RealWord.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Data.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly RealWordDbContext _context;

        public TagRepository(RealWordDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            var tags = await _context.Tags.ToListAsync();
            return tags;
        }
        public async Task<List<string>> GetTagsListAsync()
        {
            var tags = await _context.Tags.Select(t => t.TagId).ToListAsync();
            return tags;
        }
        public async Task CreateTagsAsync(List<string> tagList, Guid articleId)
        {
            foreach (var tag in tagList)
            {
                var newTag = new Tag { TagId = tag };
                await _context.Tags.AddAsync(newTag);
            }
        }
        public async Task CreateArticleTagsAsync(List<string> tagList, Guid articleId)
        {
            foreach (var tag in tagList)
            {
                var ArticleTags = new ArticleTags { TagId = tag, ArticleId = articleId };
                await _context.ArticleTags.AddAsync(ArticleTags);
            }

        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}