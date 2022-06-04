using RealWord.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealWord.Core.Repositories
{
    public interface ITagService
    {
        Task<List<Tag>> GetTagsAsync();
        void CreateTags(List<string> Tags, Guid articleId);
        Task SaveChangesAsync();
    }
}