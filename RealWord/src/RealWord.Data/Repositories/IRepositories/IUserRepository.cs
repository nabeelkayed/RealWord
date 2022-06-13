using RealWord.Data.Entities;
using System;
using System.Threading.Tasks;

namespace RealWord.Data.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UserExistsAsync(string username);
        Task<User> GetUserAsync(string username);
        Task<User> GetUserAsNoTrackingAsync(string username);
        Task<User> LoginUserAsync(User user);
        Task CreateUserAsync(User user);
        Task<bool> EmailAvailableAsync(string email);
        void UpdateUser(User updatedUser, User userForUpdate);
        Task FollowUserAsync(Guid currentUserId, Guid userToFollowId);
        void UnfollowUser(Guid currentUserId, Guid userToUnfollowId);
        Task<bool> IsFollowedAsync(Guid FollowerId, Guid FolloweingId); 
        Task SaveChangesAsync();
    }
}