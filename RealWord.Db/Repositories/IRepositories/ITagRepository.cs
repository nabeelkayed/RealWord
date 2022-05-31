using RealWord.Db.Entities;
using System;
using System.Collections.Generic;

namespace RealWord.Db.Repositories
{
    public interface ITagRepository
    {
        List<string> GetTags();
    }
}