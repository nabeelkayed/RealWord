using RealWord.Db.Entities;
using System;
using System.Collections.Generic;

namespace RealWord.Db.Repositories
{
    public interface ITagRepository
    {
        List<Tag> GetTags();
        void CreateTags(List<string> Tags, Guid articleId);
    }
}