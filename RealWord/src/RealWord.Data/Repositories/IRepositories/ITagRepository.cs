using RealWord.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealWord.Data.Repositories
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTagsAsync();
        Task<List<string>> GetTagsListAsync();
        Task CreateTagsAsync(List<string> Tags, Guid articleId);
        Task CreateArticleTagsAsync(List<string> tagList, Guid articleId);
        Task SaveChangesAsync();
    }
}