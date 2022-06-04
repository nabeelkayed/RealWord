using RealWord.Db.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealWord.Db.Repositories
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTagsAsync();
        void CreateTags(List<string> Tags, Guid articleId); 
    }
}