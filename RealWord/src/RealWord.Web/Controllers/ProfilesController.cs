using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using RealWord.Db.Repositories;
using RealWord.Web.Helpers;
using RealWord.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace RealWord.Web.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/profiles")]
    public class ProfilesController : ControllerBase
    {
        private readonly IUserRepository _IUserRepository;
        private readonly IMapper _mapper;
        private readonly IAuthentication _IAuthentication;


        public ProfilesController(IUserRepository userRepository, IAuthentication authentication,
        IMapper mapper)
        {
            _IUserRepository = userRepository ??
                         throw new ArgumentNullException(nameof(userRepository));
            _IAuthentication = authentication ??
         throw new ArgumentNullException(nameof(UserRepository));
            _mapper = mapper ??
            throw new ArgumentNullException(nameof(mapper));
        }

        [AllowAnonymous]
        [HttpGet("{username}")]
        public async Task<ActionResult<ProfileDto>> GetProfile(string username)
        {
            username = username.ToLower();
            var User = await _IUserRepository.GetUserAsync(username);
            if (User == null)
            {
                return NotFound();
            }

            var currentUser = await _IAuthentication.GetCurrentUserAsync();
            if (currentUser != null)
            {
                var ProfileToReturnlogin = _mapper.Map<ProfileDto>(User, a => a.Items["currentUserId"] = currentUser.UserId);
                return Ok(new { profile = ProfileToReturnlogin });
            }

            var ProfileToReturn = _mapper.Map<ProfileDto>(User, a => a.Items["currentUserId"] = Guid.NewGuid());
            return Ok(new { profile = ProfileToReturn });
        }

        [HttpPost("{username}/follow")]
        public async Task<ActionResult<ProfileDto>> Follow(string username)
        {
            username = username.ToLower();
            var userToFollow = await _IUserRepository.GetUserAsync(username);
            if (userToFollow == null)
            {
                return NotFound();
            }

            var currentUser = await _IAuthentication.GetCurrentUserAsync();
            if (currentUser.Username == username)
            {
                return BadRequest("You can't follow yourself");
            }

            bool isFollowed = await _IUserRepository.IsFollowedAsync(currentUser.UserId, userToFollow.UserId);
            if (isFollowed)
            {
                return BadRequest($"You already follow the user with username {username}");
            }

             _IUserRepository.FollowUser(currentUser.UserId, userToFollow.UserId);
             _IUserRepository.SaveChanges();

            var ProfileToReturn = _mapper.Map<ProfileDto>(userToFollow, a => a.Items["currentUserId"] = currentUser.UserId);
            return new ObjectResult(new { profile = ProfileToReturn }) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpDelete("{username}/follow")]
        public async Task<ActionResult<ProfileDto>> UnFollow(string username)
        {
            username = username.ToLower();
            var userToUnfollow =await _IUserRepository.GetUserAsync(username);
            if (userToUnfollow == null)
            {
                return NotFound();
            }

            var currentUser = await _IAuthentication.GetCurrentUserAsync();
            if (currentUser.Username == username)
            {
                return BadRequest("You can't unfollow yourself");
            }

            bool isFollowed = await _IUserRepository.IsFollowedAsync(currentUser.UserId, userToUnfollow.UserId);
            if (!isFollowed)
            {
                return BadRequest($"You aren't follow the user of username {username}");
            }

             _IUserRepository.UnfollowUser(currentUser.UserId, userToUnfollow.UserId);
             _IUserRepository.SaveChanges();

            var ProfileToReturn = _mapper.Map<ProfileDto>(userToUnfollow, a => a.Items["currentUserId"] = currentUser.UserId);
            return Ok(new { profile = ProfileToReturn });

        }
    }
}
