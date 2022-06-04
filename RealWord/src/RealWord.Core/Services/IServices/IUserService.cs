using RealWord.Core.Models;
using RealWord.Data.Entities;
using System;
using System.Threading.Tasks;

namespace RealWord.Core.Repositories
{
    public interface IUserService
    {
        Task<User> ValidLoginUserAsync(UserLoginDto userLogin);
        UserDto LoginUserAsync(User userLogin);
    }
}