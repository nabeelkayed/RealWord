using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealWord.Core.Models;
using System;
using System.Threading.Tasks;
using RealWord.Core.Services;

namespace RealWord.Web.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/profiles")]
    public class ProfilesController : ControllerBase
    {
        private readonly IUserService _IUserService;

        public ProfilesController(IUserService userService)
        {
            _IUserService = userService ??
                throw new ArgumentNullException(nameof(userService));
        }

        [AllowAnonymous]
        [HttpGet("{username}")]
        public async Task<ActionResult<ProfileDto>> GetProfile(string username)
        {
            var profileToReturn = await _IUserService.GetProfileAsync(username);
            if (profileToReturn == null) 
            {
                return NotFound();
            }

            return Ok(new { profile = profileToReturn });
        }

        [HttpPost("{username}/follow")]
        public async Task<ActionResult<ProfileDto>> FollowUser(string username)
        {
            var followedProfileToReturn = await _IUserService.FollowUserAsync(username);
            if (followedProfileToReturn == null)
            {
                return NotFound();
            }

            return new ObjectResult(new { profile = followedProfileToReturn }) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpDelete("{username}/follow")]
        public async Task<ActionResult<ProfileDto>> UnFollowUser(string username)
        {
            var unFollowedProfileToReturn = await _IUserService.UnFollowUserAsync(username);
            if (unFollowedProfileToReturn == null)
            {
                return NotFound();
            }

            return Ok(new { profile = unFollowedProfileToReturn });
        }
        
        [AllowAnonymous]
        [HttpOptions]
        public IActionResult ProfilesOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,Delete");
           
            return Ok();
        }
    }
}
