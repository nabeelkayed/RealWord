﻿using RealWord.Db.Entities;
using System;

namespace RealWord.Db.Repositories
{
    public interface IUserRepository
    {
        bool UserExists(string username);
        User GetUser(string username);
        User LoginUser(User user);
        void CreateUser(User user);
        void UpdateUser(User updatedUser, User userForUpdate);
        bool FollowUser(Guid currentUserId, Guid userToFollowId);
        bool UnfollowUser(Guid currentUserId, Guid userToUnfollowId);
        bool IsFollowed(Guid FollowerId, Guid FolloweingId);
        void Save();
    }
}