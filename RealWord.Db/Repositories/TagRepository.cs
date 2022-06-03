using RealWord.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealWord.Db.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly RealWordDbContext _context;

        public TagRepository(RealWordDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public List<Tag> GetTags()
        {
            var tags = _context.Tags.ToList();
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
    }
}