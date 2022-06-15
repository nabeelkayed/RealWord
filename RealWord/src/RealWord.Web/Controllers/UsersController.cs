using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using RealWord.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using RealWord.Core.Services;

namespace RealWord.Web.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _IUserService;

        public UsersController(IUserService userService)
        {
            _IUserService = userService ??
               throw new ArgumentNullException(nameof(userService));
        }

        [AllowAnonymous]
        [HttpPost("users/login")]
        public async Task<ActionResult<UserDto>> Login(UserLoginDto userLogin)
        {
            var logedinUserToReturn =await _IUserService.LoginUserAsync(userLogin);
            if (logedinUserToReturn == null)
            {
                return NotFound();
            }
         
            return Ok(new { user = logedinUserToReturn });
        }

        [AllowAnonymous]
        [HttpPost("users")]
        public async Task<ActionResult<UserDto>> CreateUser(UserForCreationDto userForCreation)
        {
            var CreatedUserToReturn = await _IUserService.CreateUserAsync(userForCreation);
            if (CreatedUserToReturn == null)
            {
                return NotFound();
            }

            CreatedUserToReturn.Token = await HttpContext.GetTokenAsync("access_token");
            return new ObjectResult(new { user = CreatedUserToReturn }) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet("user")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userToReturn = await _IUserService.GetCurrentUserAsync();
            if (userToReturn == null)
            {
                return Unauthorized();
            }

            userToReturn.Token = await HttpContext.GetTokenAsync("access_token");
            return Ok(new { user = userToReturn });
        }

        [HttpPut("user")]
        public async Task<ActionResult<UserDto>> UpdateUser(UserForUpdateDto userForUpdate)
        {
            var updatedUserToReturn = await _IUserService.UpdateUserAsync(userForUpdate);
            if (updatedUserToReturn == null)
            {
                return NotFound();
            }
            updatedUserToReturn.Token = await HttpContext.GetTokenAsync("access_token");
            return Ok(new { user = updatedUserToReturn });
        }

        [AllowAnonymous]
        [HttpOptions("user")]
        public IActionResult UserOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,PUT");

            return Ok();
        }

        [AllowAnonymous] 
        [HttpOptions("users")]
        public IActionResult UsersOptions()
        {
            Response.Headers.Add("Allow", "OPTIONS,POST");

            return Ok();
        }
    }
}