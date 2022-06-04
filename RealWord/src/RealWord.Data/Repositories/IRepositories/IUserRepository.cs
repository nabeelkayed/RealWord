﻿using RealWord.Db.Entities;
using System;
using System.Threading.Tasks;

namespace RealWord.Db.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UserExistsAsync(string username);
        Task<User> GetUserAsync(string username);
        User GetUser(string username);

        Task<User> LoginUserAsync(User user);
        void CreateUser(User user);
        Task<bool> EmailAvailableAsync(string email);
        void UpdateUser(User updatedUser, User userForUpdate);
        void FollowUser(Guid currentUserId, Guid userToFollowId);
        void UnfollowUser(Guid currentUserId, Guid userToUnfollowId);
        Task<bool> IsFollowedAsync(Guid FollowerId, Guid FolloweingId); 
        void SaveChanges();
    }
}