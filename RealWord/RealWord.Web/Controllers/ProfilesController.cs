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
            var user = _IUserRepository.GetUser(username);

            if (user == null)
            {
                return NotFound();
            }

            var profile = _mapper.Map<ProfileDto>(user);

            var currUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (currUsername == null)
            {
                return Ok(new { profile = profile });

            }
            
            var user1 = _IUserRepository.GetUser(currUsername);
            var Isfolo = _IUserRepository.Isfolo(user1, user);
            profile.following = Isfolo;

            if (currUsername == username)
            {
                profile.following = false;
            }

            return Ok(new { profile = profile });
        }

        [HttpPost("{username}/follow")]
        public ActionResult<ProfileDto> Follow(string username)
        {
            var user = _IUserRepository.GetUser(username);

            if (user == null)
            {
                return NotFound();
            }

            var currUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (currUsername == username)
            {
                return BadRequest("You can't follow yourself");
            }
            var currUser = _IUserRepository.GetUser(currUsername);

            var ff = _IUserRepository.FollowUser(currUser, user);
            if (ff)
            {
                _IUserRepository.Save();
                var userprofile = _mapper.Map<ProfileDto>(user);
                userprofile.following = true;
                return Ok(new { profile = _mapper.Map<ProfileDto>(userprofile) });

            }
            return BadRequest($"You alredy follow the user of username {username}");
        }

        [HttpDelete("{username}/follow")]
        public ActionResult<ProfileDto> UnFollow(string username)
        {
            var user = _IUserRepository.GetUser(username);

            if (user == null)
            {
                return NotFound();
            }

            var currUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (currUsername == username)
            {
                return BadRequest("You can't unfollow yourself");
            }

            var currUser = _IUserRepository.GetUser(currUsername);

            var ff = _IUserRepository.UnFollowUser(currUser, user);
            if (ff)
            {
                _IUserRepository.Save();
                var userprofile = _mapper.Map<ProfileDto>(user);
                userprofile.following = false;
                return Ok(new { profile = _mapper.Map<ProfileDto>(userprofile) });
            }
            return BadRequest($"You are not follow the user of username {username}");

        }
    }
}
