using Microsoft.EntityFrameworkCore;
using RealWord.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public void CreateTags(List<string> tagList, Guid articleId)
        {
            var tags = _context.Tags.Select(t => t.TagId).ToList();
            foreach (var tag in tagList)
            {
                if (!tags.Contains(tag))
                {
                    var newTag = new Tag { TagId = tag };
                    _context.Tags.Add(newTag);
                }

                var ArticleTags = new ArticleTags { TagId = tag, ArticleId = articleId };
                _context.ArticleTags.Add(ArticleTags);
            }
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}