using Microsoft.AspNetCore.Http;
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
        public User GetUser(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }
        public User LoginUser(User User)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == User.Email && u.Password == User.Password);
            return user;
        }
        public void CreateUser(User user)
        {
            _context.Users.Add(user);
        }
        public User UpdateUser(User CurrUser, User User)
        {
            var UpdatedUser = _context.Users.FirstOrDefault(a => a.Username == CurrUser.Username);

            if (!string.IsNullOrWhiteSpace(User.Email))
            {
                UpdatedUser.Email = User.Email;
            }

            if (!string.IsNullOrWhiteSpace(User.Image))
            {
                UpdatedUser.Image = User.Image;
            }

            if (!string.IsNullOrWhiteSpace(User.Bio))
            {
                UpdatedUser.Bio = User.Bio;
            }
            if (!string.IsNullOrWhiteSpace(User.Password))
            {
                UpdatedUser.Password = User.Password;
            }
            if (!string.IsNullOrWhiteSpace(User.Username))
            {
                UpdatedUser.Username = User.Username;
            }
            return UpdatedUser;
        }
        public bool FollowUser(User CurrentUser, User User)
        {
            if (IsFollow(CurrentUser, User))
            {
                return false;
            }
            var UserFollower = new UserFollowers { FollowerId = CurrentUser.UserId, FolloweingId = User.UserId };
            _context.UserFollowers.Add(UserFollower);
            return true;
        }

        public bool IsFollow(User CurrentUser, User user)
        {
            return _context.UserFollowers.Any(uf => uf.FollowerId == CurrentUser.UserId
                        && uf.FolloweingId == user.UserId);
        }

        public bool UnfollowUser(User CurrentUser, User User)
        {
            if (!IsFollow(CurrentUser, User)) 
            {
                return false;
            }

            var UserFollower = new UserFollowers { FollowerId = CurrentUser.UserId, FolloweingId = User.UserId };
            _context.UserFollowers.Remove(UserFollower);

            return true;
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public bool UserExists(string Username)
        {
            if (String.IsNullOrEmpty(Username))
            {
                throw new ArgumentNullException(nameof(Username));
            }

            return _context.Users.Any(a => a.Username == Username);
        }
    }
}