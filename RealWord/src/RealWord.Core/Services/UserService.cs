using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealWord.Data;
using RealWord.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWord.Core.Repositories
{
    public class UserService : IUserService
    {
        private readonly RealWordDbContext _context;

        public UserService(RealWordDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<bool> UserExistsAsync(string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            bool userExists = await _context.Users.AnyAsync(u => u.Username == username);
            return userExists;
        }
        public async Task<User> GetUserAsync(string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            var user = await _context.Users.Include(a => a.Articles).FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }
        public async Task<User> LoginUserAsync(User user)
        {
            var LoginUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email
                                                             && u.Password == user.Password);
            return LoginUser;
        }
        public void CreateUser(User user)
        {
            _context.Users.Add(user);
        }
        public async Task<bool> EmailAvailableAsync(string email)
        {
            var emailAvailable = await _context.Users.Select(a => a.Email).ContainsAsync(email);
            return emailAvailable;
        }
        public void UpdateUser(User updatedUser, User userForUpdate)
        {
        }
        public void FollowUser(Guid currentUserId, Guid userToFollowId)
        {
            var userFollower =
                new UserFollowers { FollowerId = currentUserId, FolloweingId = userToFollowId };
            _context.UserFollowers.Add(userFollower);
        }
        public void UnfollowUser(Guid currentUserId, Guid userToUnfollowId)
        {
            var userFollower =
                new UserFollowers { FollowerId = currentUserId, FolloweingId = userToUnfollowId };
            _context.UserFollowers.Remove(userFollower);
        }
        public async Task<bool> IsFollowedAsync(Guid FollowerId, Guid FolloweingId)
        {
            bool isFollowed =
               await _context.UserFollowers.AnyAsync(uf => uf.FollowerId == FollowerId && uf.FolloweingId == FolloweingId);
            return isFollowed;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}