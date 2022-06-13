using RealWord.Core.Models;
using System;
using System.Threading.Tasks;

namespace RealWord.Core.Services
{
    public interface IUserService
    {
        Task<UserDto> LoginUserAsync(UserLoginDto userLogin);
        Task<UserDto> GetCurrentUserAsync();
        Task<Guid> GetCurrentUserIdAsync();
        Task<UserDto> CreateUserAsync(UserForCreationDto userForCreation);
        Task<UserDto> UpdateUserAsync(UserForUpdateDto userForUpdateDto);
        Task<ProfileDto> GetProfileAsync(string username);
        Task<ProfileDto> FollowUserAsync(string username);
        Task<ProfileDto> UnFollowUserAsync(string username);
    }
}