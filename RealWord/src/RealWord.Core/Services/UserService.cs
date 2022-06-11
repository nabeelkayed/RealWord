using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealWord.Core.Auth;
using RealWord.Core.Models;
using RealWord.Data;
using RealWord.Data.Entities;
using RealWord.Data.Repositories;
using RealWord.Utils.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RealWord.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _IUserRepository;
        private readonly IAuthentication _IAuthentication;
        private readonly IHttpContextAccessor _accessor;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IAuthentication authentication,
            IHttpContextAccessor accessor, IMapper mapper)
        {
            _IUserRepository = userRepository ??
                throw new ArgumentNullException(nameof(UserRepository));
            _IAuthentication = authentication ??
                throw new ArgumentNullException(nameof(UserRepository));
            _accessor = accessor ??
                throw new ArgumentNullException(nameof(accessor));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<UserDto>  LoginUserAsync(UserLoginDto userLogin)
        {
            userLogin.Email = userLogin.Email.ToLower();
            //userLogin.Password.GetHash(); 

            var user1 = _mapper.Map<User>(userLogin);
            var user = await _IUserRepository.LoginUserAsync(user1);
            if (user == null)
            {
                return null;
            }

            var userToReturn = _mapper.Map<UserDto>(user);
            userToReturn.Token = _IAuthentication.Generate(user);
            return userToReturn;
        }
        public async Task<UserDto> GetCurrentUserAsync()
        {
            var currentUsername = _accessor?.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!String.IsNullOrEmpty(currentUsername))
            {
                var currentUser = await _IUserRepository.GetUserAsync(currentUsername);
                var userToReturn = _mapper.Map<UserDto>(currentUser);
                return userToReturn;
            }

            return null;
        }
        public async Task<Guid> GetCurrentUserIdAsync()
        {
            var currentUsername = _accessor?.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!String.IsNullOrEmpty(currentUsername))
            {
                var currentUser = await _IUserRepository.GetUserAsync(currentUsername);
                return currentUser.UserId;
            }

            return Guid.Empty;
        }
        public async Task<UserDto> CreateUserAsync(UserForCreationDto userForCreation)
        {
            userForCreation.Username = userForCreation.Username.ToLower();
            userForCreation.Email = userForCreation.Email.ToLower();
            userForCreation.Password.GetHash();

            var userExists = await _IUserRepository.UserExistsAsync(userForCreation.Username);
            if (userExists)
            {
                return null;
            }

            var emailAvailable = await _IUserRepository.EmailAvailableAsync(userForCreation.Email);
            if (!emailAvailable)
            {
                return null;
            }

            var userEntityForCreation = _mapper.Map<User>(userForCreation);
            _IUserRepository.CreateUser(userEntityForCreation);
            await _IUserRepository.SaveChangesAsync();

            var userToReturn = _mapper.Map<UserDto>(userEntityForCreation);
            return userToReturn;
        }
        public async Task<UserDto> UpdateUserAsync(UserForUpdateDto userForUpdate)
        {
            var currentUser1 = await GetCurrentUserAsync();
            var currentUser = await _IUserRepository.GetUserAsync(currentUser1.Username);

            var userEntityForUpdate = _mapper.Map<User>(userForUpdate);

            if (!string.IsNullOrWhiteSpace(userForUpdate.Email))
            {
                currentUser.Email = userForUpdate.Email.ToLower();
            }
            if (!string.IsNullOrWhiteSpace(userForUpdate.Image))
            {
                currentUser.Image = userForUpdate.Image;
            }
            if (!string.IsNullOrWhiteSpace(userForUpdate.Bio))
            {
                currentUser.Bio = userForUpdate.Bio;
            }
            if (!string.IsNullOrWhiteSpace(userForUpdate.Password))
            {
                currentUser.Password = userForUpdate.Password.GetHash();
            }
            if (!string.IsNullOrWhiteSpace(userForUpdate.Username))
            {
                currentUser.Username = userForUpdate.Username.ToLower();
            }

            _IUserRepository.UpdateUser(currentUser, userEntityForUpdate);
            await _IUserRepository.SaveChangesAsync();

            var userToReturn = _mapper.Map<UserDto>(currentUser);
            return userToReturn;
        }
        public async Task<ProfileDto> GetProfileAsync(string username)
        {
            username = username.ToLower();
         
            var User = await _IUserRepository.GetUserAsync(username);
            if (User == null)
            {
                return null;
            }

            var currentUserId = await GetCurrentUserIdAsync();
            if (currentUserId != null)
            {
                var profileToReturnlogin = _mapper.Map<ProfileDto>(User, a => a.Items["currentUserId"] = currentUserId);
                return profileToReturnlogin;
            }

            var profileToReturn = _mapper.Map<ProfileDto>(User, a => a.Items["currentUserId"] = Guid.NewGuid());
            return profileToReturn;
        }
        public async Task<ProfileDto> FollowUserAsync(string username)
        {
            username = username.ToLower();
            
            var userToFollow = await _IUserRepository.GetUserAsync(username);
            if (userToFollow == null)
            {
                return null;
            }

            var currentUserDto = await GetCurrentUserAsync();
            var currentUserId = await GetCurrentUserIdAsync();
            if (currentUserDto.Username == username)
            {
                return null;
            }

            bool isFollowed = await _IUserRepository.IsFollowedAsync(currentUserId, userToFollow.UserId);
            if (isFollowed)
            {
                return null;
            }

            _IUserRepository.FollowUser(currentUserId, userToFollow.UserId);
            await _IUserRepository.SaveChangesAsync();

            var profileToReturn = _mapper.Map<ProfileDto>(userToFollow, a => a.Items["currentUserId"] = currentUserId);
            return profileToReturn;
        }

        public async Task<ProfileDto> UnFollowUserAsync(string username)
        {
            username = username.ToLower();
            
            var userToUnfollow = await _IUserRepository.GetUserAsync(username);
            if (userToUnfollow == null)
            {
                return null;
            }

            var currentUserDto = await GetCurrentUserAsync();
            var currentUserId = await GetCurrentUserIdAsync();
            if (currentUserDto.Username == username)
            {
                return null;
            }

            bool isFollowed = await _IUserRepository.IsFollowedAsync(currentUserId, userToUnfollow.UserId);
            if (!isFollowed)
            {
                return null;
            }

            _IUserRepository.UnfollowUser(currentUserId, userToUnfollow.UserId);
            await _IUserRepository.SaveChangesAsync();

            var profileToReturn = _mapper.Map<ProfileDto>(userToUnfollow, a => a.Items["currentUserId"] = currentUserId);
            return profileToReturn;
        }
    }
}