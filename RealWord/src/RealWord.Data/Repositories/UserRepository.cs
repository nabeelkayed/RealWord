using Microsoft.EntityFrameworkCore;
using RealWord.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RealWordDbContext _context;

        public UserRepository(RealWordDbContext context)
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

            var user = await _context.Users.Include(u => u.Articles)
                                           .Include(u => u.Followerings)
                                           .Include(u => u.Followers)
                                           .FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }
        public async Task<User> GetUserAsNoTrackingAsync(string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            var user = await _context.Users.AsNoTracking().Include(u => u.Articles)
                                           .Include(u => u.Followerings)
                                           .Include(u => u.Followers)
                                           .FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }
        public async Task<User> LoginUserAsync(User user)
        {
            var LoginUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email
                                                             && u.Password == user.Password);
            return LoginUser;
        }
        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }
        public async Task<bool> EmailAvailableAsync(string email)
        {
            var emailNotAvailable = await _context.Users.Select(a => a.Email).ContainsAsync(email);
            return emailNotAvailable;
        }
        public void UpdateUser(User updatedUser, User userForUpdate)
        {
        }
        public async Task FollowUserAsync(Guid currentUserId, Guid userToFollowId)
        {
            var userFollower =
                new UserFollowers { FollowerId = currentUserId, FolloweingId = userToFollowId };
            await _context.UserFollowers.AddAsync(userFollower);
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