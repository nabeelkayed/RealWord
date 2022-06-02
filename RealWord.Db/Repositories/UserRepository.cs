using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealWord.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealWord.Db.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RealWordDbContext _context;

        public UserRepository(RealWordDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public bool UserExists(string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            bool userExists = _context.Users.Any(u => u.Username == username);
            return userExists;
        }
        public User GetUser(string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            var user = _context.Users.Include(a=>a.Articles).FirstOrDefault(u => u.Username == username);
            return user;
        }
        public  User LoginUser(User user)
        {
            var LoginUser = _context.Users.FirstOrDefault(u => u.Email == user.Email
                                                            && u.Password == user.Password);
            return LoginUser;
        }
        public void CreateUser(User user)
        { 
            var Emails = _context.Users.Select(a => a.Email).ToList();//يطلع برة 
            if (Emails.Contains(user.Email))
            {
                throw new ArgumentNullException(nameof(user.Email));
            }

            _context.Users.Add(user);
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
        public bool IsFollowed(Guid FollowerId, Guid FolloweingId)
        {
            bool isFollowed =
                _context.UserFollowers.Any(uf => uf.FollowerId == FollowerId && uf.FolloweingId == FolloweingId);
            return isFollowed;
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}