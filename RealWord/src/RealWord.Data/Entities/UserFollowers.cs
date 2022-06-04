using System;
using System.Collections.Generic;
using System.Text;

namespace RealWord.Data.Entities
{
    public class UserFollowers
    {
        public Guid FollowerId { get; set; }
        public Guid FolloweingId { get; set; }

        public User Follower { get; set; }
        public User Followeing { get; set; }
    }
}