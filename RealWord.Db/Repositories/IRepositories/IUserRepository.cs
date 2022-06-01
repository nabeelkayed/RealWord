using RealWord.Db.Entities;
using System;

namespace RealWord.Db.Repositories
{
    public interface IUserRepository
    {
        bool UserExists(string username);
        User GetUser(string username);
        User LoginUser(User user);
        void CreateUser(User user);
        User UpdateUser(string currentUsername, User userForUpdate);//تصغير الإسم
        bool FollowUser(Guid currentUserId, Guid userToFollowId);
        bool UnfollowUser(Guid currentUserId, Guid userToUnfollowId);
        public bool IsFollowed(Guid FollowerId, Guid FolloweingId);
        void Save();
    }
}