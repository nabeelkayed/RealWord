using RealWord.Db.Entities;
using System;

namespace RealWord.Db.Repositories
{
    public interface IUserRepository
    {
        User GetUser(string Username);
        User LoginUser(User User);
        void CreateUser(User User);
        User UpdateUser(User CurrentUser,User User); 
        bool FollowUser(User CurrentUser, User User); 
        bool UnfollowUser(User CurrentUser, User User); 
        public bool IsFollow(User CurrentUser, User User);
        bool UserExists(string Username);
        void Save();
    }
}