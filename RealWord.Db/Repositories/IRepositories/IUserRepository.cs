using RealWord.Db.Entities;
using System;

namespace RealWord.Db.Repositories
{
    public interface IUserRepository
    {
        User GetUser(string username);
        User LoginUser(User user);
        void CreateUser(User user);
        User UpdateUser(User currUser,User user); 
        bool FollowUser(User currUser, User user);
        bool UnFollowUser(User currUser, User user);
        public bool Isfolo(User currUser, User user);
        void Save();
    }
}