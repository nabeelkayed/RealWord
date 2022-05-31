using RealWord.Db.Entities;
using System;

namespace RealWord.Db.Repositories
{
    public interface IUserRepository
    {
        bool UserExists(string Username);
        User LoginUser(User User);
        void CreateUser(User User);
        User GetUser(string Username);
        User UpdateUser(User CurrentUser,User User); 
        bool FollowUser(User CurrentUser, User User); 
        bool UnfollowUser(User CurrentUser, User User); 
        public bool IsFollow(User CurrentUser, User User);
        void Save();
    }
}