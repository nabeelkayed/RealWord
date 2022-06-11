using RealWord.Core.Models;
using RealWord.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealWord.Core.Services
{
    public interface ITagService
    {
        Task<TagDto> GetTagsAsync();
        Task CreateTags(List<string> tagList, Guid articleId);
    }
}