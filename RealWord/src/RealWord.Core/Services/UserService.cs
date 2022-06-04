using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealWord.Core.Auth;
using RealWord.Core.Models;
using RealWord.Data;
using RealWord.Data.Entities;
using RealWord.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWord.Core.Repositories
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _IUserRepository;
        private readonly IMapper _mapper;
        private readonly IAuthentication _IAuthentication;

        public UserService(IUserRepository userRepository, IAuthentication authentication, IMapper mapper)
        {
            _IUserRepository = userRepository ??
                throw new ArgumentNullException(nameof(UserRepository));
            _IAuthentication = authentication ??
                   throw new ArgumentNullException(nameof(UserRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<User> ValidLoginUserAsync(UserLoginDto userLogin)
        {
            userLogin.Email = userLogin.Email.ToLower();
            //userLogin.Password.GetHash(); 

            var user1 = _mapper.Map<User>(userLogin);
            var user = await _IUserRepository.LoginUserAsync(user1);
            return user;
        }
        public UserDto LoginUserAsync(User userLogin)
        {
            var userToReturn = _mapper.Map<UserDto>(userLogin);
            userToReturn.Token = _IAuthentication.Generate(userLogin);
            return userToReturn;
        }
    }
}