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
    }
}