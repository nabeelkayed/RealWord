using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using RealWord.Db.Repositories;
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

        public ProfilesController(IUserRepository userRepository,
            IMapper mapper)
        {
            _IUserRepository = userRepository ??
                         throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [AllowAnonymous]
        [HttpGet("{username}")]
        public ActionResult<ProfileDto> GetProfile(string username)
        {
            var User = _IUserRepository.GetUser(username);
            if (User == null)
            {
                return NotFound();
            }

            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (CurrentUsername != null)
            {
                var CurrentUserId = _IUserRepository.GetUser(CurrentUsername).UserId;
                var ProfileToReturnlogin = _mapper.Map<ProfileDto>(User, a => a.Items["currentUserId"] = CurrentUserId);
                return Ok(new { profile = ProfileToReturnlogin });
            }

            var ProfileToReturn = _mapper.Map<ProfileDto>(User, a => a.Items["currentUserId"] = Guid.NewGuid());
            return Ok(new { profile = ProfileToReturn });
        }

        [HttpPost("{username}/follow")]
        public ActionResult<ProfileDto> Follow(string username)
        {
            var userToFollow = _IUserRepository.GetUser(username);
            if (userToFollow == null)
            {
                return NotFound();
            }

            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (CurrentUsername == username)
            {
                return BadRequest("You can't follow yourself");
            }
            var currentUserId = _IUserRepository.GetUser(CurrentUsername).UserId;

            bool isFollowed = _IUserRepository.IsFollowed(currentUserId, userToFollow.UserId);
            if (isFollowed)
            {
                return BadRequest($"You already follow the user with username {username}");
            }

            _IUserRepository.FollowUser(currentUserId, userToFollow.UserId);
            _IUserRepository.Save();

            var ProfileToReturn = _mapper.Map<ProfileDto>(User, a => a.Items["currentUserId"] = currentUserId);
            return Ok(new { profile = ProfileToReturn });
        }

        [HttpDelete("{username}/follow")]
        public ActionResult<ProfileDto> UnFollow(string username)
        {
            var userToUnfollow = _IUserRepository.GetUser(username);
            if (userToUnfollow == null)
            {
                return NotFound();
            }

            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (CurrentUsername == username)
            {
                return BadRequest("You can't unfollow yourself");
            }
            var currentUserId = _IUserRepository.GetUser(CurrentUsername).UserId;

            bool isUnfollowed = !_IUserRepository.IsFollowed(currentUserId, userToUnfollow.UserId);

            if (isUnfollowed)
            {
                return BadRequest($"You aren't follow the user of username {username}");
            }

            _IUserRepository.UnfollowUser(currentUserId, userToUnfollow.UserId);
            _IUserRepository.Save();

            var ProfileToReturn = _mapper.Map<ProfileDto>(User, a => a.Items["currentUserId"] = currentUserId);
            return Ok(new { profile = ProfileToReturn });

        }
    }
}
