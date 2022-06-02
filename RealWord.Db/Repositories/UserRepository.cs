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
            var Emails = _context.Users.Select(a => a.Email).ToList();
            if (Emails.Contains(user.Email))
            {
                throw new ArgumentNullException(nameof(user.Email));
            }

            _context.Users.Add(user);
        }
        public void UpdateUser(User updatedUser, User userForUpdate)
        {
            if (!string.IsNullOrWhiteSpace(userForUpdate.Email))
            {
                updatedUser.Email = userForUpdate.Email;
            }
            if (!string.IsNullOrWhiteSpace(userForUpdate.Image))
            {
                updatedUser.Image = userForUpdate.Image;
            }
            if (!string.IsNullOrWhiteSpace(userForUpdate.Bio))
            {
                updatedUser.Bio = userForUpdate.Bio;
            }
            if (!string.IsNullOrWhiteSpace(userForUpdate.Password))
            {
                updatedUser.Password = userForUpdate.Password;
            }
            if (!string.IsNullOrWhiteSpace(userForUpdate.Username))
            {
                updatedUser.Username = userForUpdate.Username;
            }
        }
        public bool FollowUser(Guid currentUserId, Guid userToFollowId)
        {
            bool isFollowed = IsFollowed(currentUserId, userToFollowId);
            if (isFollowed)
            {
                return false;
            }

            var userFollower =
                new UserFollowers { FollowerId = currentUserId, FolloweingId = userToFollowId };
            _context.UserFollowers.Add(userFollower);
            return true;
        }
        public bool UnfollowUser(Guid currentUserId, Guid userToUnfollowId)
        {
            bool isUnfollowed = !IsFollowed(currentUserId, userToUnfollowId);
            if (isUnfollowed)
            {
                return false;
            }

            var userFollower =
                new UserFollowers { FollowerId = currentUserId, FolloweingId = userToUnfollowId };
            _context.UserFollowers.Remove(userFollower);
            return true;
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