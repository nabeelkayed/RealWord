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
        public User LoginUser(User user)
        {
            var a = _context.Users.FirstOrDefault(o => o.Email == user.Email && o.Password == user.Password);
            return a;
        }
        public void CreateUser(User user)
        {
            _context.Users.Add(user);
        }
        public void UpdateUser(User User)
        {
            /*var user = _context.Users.Single(u=>u.Username == usrname);
            //Author.AuthorName += "Kayed";
            return user;*/
        }
        public bool FollowUser(User currUser, User user)
        {
            bool ff = Isfolo(currUser, user);
            if (ff)
            {
                return false;
            }
            var UserFollower = new UserFollowers { FollowerId = currUser.UserId, FolloweingId = user.UserId };
            _context.UserFollowers.Add(UserFollower);
            return true;
        }

        public bool Isfolo(User currUser, User user)
        {
            return _context.UserFollowers.Any(uf => uf.FollowerId == currUser.UserId
                        && uf.FolloweingId == user.UserId);
        }

        public bool UnFollowUser(User currUser, User user)
        {
            var ff = _context.UserFollowers.Any(uf => uf.FollowerId == currUser.UserId
           && uf.FolloweingId == user.UserId);
            if (!ff)
            {
                return false;
            }
            var UserFollower = new UserFollowers { FollowerId = currUser.UserId, FolloweingId = user.UserId };
            _context.UserFollowers.Remove(UserFollower);
            return true;
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}