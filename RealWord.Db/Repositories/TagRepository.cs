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
        public List<string> GetTags()
        {
            return _context.Tags.Select(t => t.TagId).ToList();
        }
    }
}