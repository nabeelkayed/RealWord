using RealWord.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Models
{
    public class CommentDto
    {
        public Guid id { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string body { get; set; }
        public ProfileDto author { get; set; }
    }
}
 