using AutoMapper;
using Microsoft.AspNetCore.Http;
using RealWord.Core.Auth;
using RealWord.Core.Models;
using RealWord.Data.Entities;
using RealWord.Data.Repositories;
using RealWord.Utils.Utils;
using System;
using System.Linq;
using System.Security.Claims;
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
        public async Task<UserDto> LoginUserAsync(UserLoginDto userLogin)
        {
            userLogin.Email = userLogin.Email.ToLower();
            userLogin.Password = userLogin.Password.GetHash();

            var user = _mapper.Map<User>(userLogin);
            var userlogedin = await _IUserRepository.LoginUserAsync(user);
            if (userlogedin == null)
            {
                return null;
            }

            var userToReturn = _mapper.Map<UserDto>(userlogedin);
            userToReturn.Token = _IAuthentication.Generate(userlogedin);
            return userToReturn;
        }
        public async Task<UserDto> GetCurrentUserAsync()
        {
            var currentUsername = _accessor?.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!String.IsNullOrEmpty(currentUsername))
            {
                var currentUser = await _IUserRepository.GetUserAsNoTrackingAsync(currentUsername);
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
                var currentUser = await _IUserRepository.GetUserAsNoTrackingAsync(currentUsername);
                return currentUser.UserId;
            }

            return Guid.Empty;
        }
        public async Task<UserDto> CreateUserAsync(UserForCreationDto userForCreation)
        {
            userForCreation.Username = userForCreation.Username.ToLower();
            userForCreation.Email = userForCreation.Email.ToLower();
            userForCreation.Password = userForCreation.Password.GetHash();

            var userExists = await _IUserRepository.UserExistsAsync(userForCreation.Username);
            if (userExists)
            {
                return null;
            }

            var emailNotAvailable = await _IUserRepository.EmailAvailableAsync(userForCreation.Email);
            if (emailNotAvailable)
            {
                return null;
            }

            var userEntityForCreation = _mapper.Map<User>(userForCreation);
            await _IUserRepository.CreateUserAsync(userEntityForCreation);
            await _IUserRepository.SaveChangesAsync();
             
            var createdUserToReturn = _mapper.Map<UserDto>(userEntityForCreation);
            return createdUserToReturn;
        }
        public async Task<UserDto> UpdateUserAsync(UserForUpdateDto userForUpdate)
        {
            var currentUser = await GetCurrentUserAsync();
            var updatedUser = await _IUserRepository.GetUserAsync(currentUser.Username);

            var userEntityForUpdate = _mapper.Map<User>(userForUpdate);

            if (!string.IsNullOrWhiteSpace(userForUpdate.Email))
            {
                var EmailNotAvailable = await _IUserRepository.EmailAvailableAsync(userForUpdate.Email);
                if (EmailNotAvailable)
                {
                    return null;
                }

                updatedUser.Email = userForUpdate.Email.ToLower();
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
                updatedUser.Password = userForUpdate.Password.GetHash();
            }
            if (!string.IsNullOrWhiteSpace(userForUpdate.Username))
            {
                updatedUser.Username = userForUpdate.Username.ToLower();
            }

            _IUserRepository.UpdateUser(updatedUser, userEntityForUpdate);
            await _IUserRepository.SaveChangesAsync();

            var UpdatedUserToReturn = _mapper.Map<UserDto>(updatedUser);
            return UpdatedUserToReturn;
        }
        public async Task<ProfileDto> GetProfileAsync(string username)
        {
            username = username.ToLower();

            var user = await _IUserRepository.GetUserAsync(username);
            if (user == null)
            {
                return null;
            }

            var currentUserId = await GetCurrentUserIdAsync();

            var profileToReturn = _mapper.Map<ProfileDto>(user, a => a.Items["currentUserId"] = currentUserId);
            return profileToReturn;
        }
        public async Task<ProfileDto> FollowUserAsync(string username)
        {
            username = username.ToLower();

            var currentUser = await GetCurrentUserAsync();
            var currentUserId = await GetCurrentUserIdAsync();
            if (currentUser.Username == username)
            {
                return null;
            }

            var userToFollow = await _IUserRepository.GetUserAsync(username);
            if (userToFollow == null)
            {
                return null;
            }

            bool isFollowed = await _IUserRepository.IsFollowedAsync(currentUserId, userToFollow.UserId);
            if (isFollowed)
            {
                return null;
            }

            await _IUserRepository.FollowUserAsync(currentUserId, userToFollow.UserId);
            await _IUserRepository.SaveChangesAsync();

            var FollowedprofileToReturn = _mapper.Map<ProfileDto>(userToFollow, a => a.Items["currentUserId"] = currentUserId);
            return FollowedprofileToReturn;
        }

        public async Task<ProfileDto> UnFollowUserAsync(string username)
        {
            username = username.ToLower();

            var currentUser = await GetCurrentUserAsync();
            var currentUserId = await GetCurrentUserIdAsync();
            if (currentUser.Username == username)
            {
                return null;
            }

            var userToUnfollow = await _IUserRepository.GetUserAsNoTrackingAsync(username);
            if (userToUnfollow == null)
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

            var UnfollowedUser = await _IUserRepository.GetUserAsync(username);
            
            var unfollowedProfileToReturn = _mapper.Map<ProfileDto>(UnfollowedUser, a => a.Items["currentUserId"] = currentUserId);
            return unfollowedProfileToReturn;
        }
    }
}