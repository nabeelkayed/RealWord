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
        bool EmailAvailable(string email);
        void UpdateUser(User updatedUser, User userForUpdate);
        void FollowUser(Guid currentUserId, Guid userToFollowId);
        void UnfollowUser(Guid currentUserId, Guid userToUnfollowId);
        bool IsFollowed(Guid FollowerId, Guid FolloweingId);
        void Save();
    }
}