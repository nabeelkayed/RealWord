using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealWord.Web.Models;
using RealWord.Db.Repositories;
using RealWord.Db.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using RealWord.Web.Helpers;
using Microsoft.Net.Http.Headers;
using RealWord.Utils.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace RealWord.Web.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _IUserRepository;
        private readonly IAuthentication _IAuthentication;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;


        public UsersController(IUserRepository userRepository, IAuthentication authentication,
            IConfiguration config, IMapper mapper)
        {
            _IUserRepository = userRepository ??
                throw new ArgumentNullException(nameof(UserRepository));
            _IAuthentication = authentication ??
                    throw new ArgumentNullException(nameof(UserRepository));
            _config = config ??
                throw new ArgumentNullException(nameof(config));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [AllowAnonymous]
        [HttpPost("users/login")]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            userLogin.Email = userLogin.Email.ToLower();
            //userLogin.Password.GetHash(); 

            var userLogedin = await _IAuthentication.LoginUserAsync(userLogin);
            if (userLogedin == null)
            {
                return NotFound();
            }

            var userToReturn = _mapper.Map<UserDto>(userLogedin);
            userToReturn.Token = _IAuthentication.Generate(userLogedin);
            return Ok(new { user = userToReturn });
        }

        [AllowAnonymous]
        [HttpPost("users")]
        public async Task<ActionResult<UserDto>> CreateUser(UserForCreationDto userForCreation)
        {
            userForCreation.Username = userForCreation.Username.ToLower();
            userForCreation.Password.GetHash();

            var userExists = await _IUserRepository.UserExistsAsync(userForCreation.Username);
            if (userExists)
            {
                return NotFound("The user is exist");
            }

            userForCreation.Email = userForCreation.Email.ToLower();
            var emailAvailable = await _IUserRepository.EmailAvailableAsync(userForCreation.Email);
            if (!emailAvailable)
            {
                return NotFound("The user is exist");
            }

            var userEntityForCreation = _mapper.Map<User>(userForCreation);
            _IUserRepository.CreateUser(userEntityForCreation);
            _IUserRepository.SaveChanges();

            var userToReturn = _mapper.Map<UserDto>(userEntityForCreation);
            userToReturn.Token = await HttpContext.GetTokenAsync("access_token");
            return new ObjectResult(new { user = userToReturn }) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet("user")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var currentUser = _IAuthentication.GetCurrentUserAsync();

            var userToReturn = _mapper.Map<UserDto>(currentUser);
            userToReturn.Token = await HttpContext.GetTokenAsync("access_token");
            return Ok(new { user = userToReturn });
        }

        [HttpPut("user")]
        public async Task<ActionResult<UserDto>> UpdateUser(UserForUpdateDto userForUpdate)
        {
            var currentUser = await _IAuthentication.GetCurrentUserAsync();

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
            _IUserRepository.SaveChanges();

            var userToReturn = _mapper.Map<UserDto>(currentUser);
            userToReturn.Token = await HttpContext.GetTokenAsync("access_token");
            return Ok(new { user = userToReturn });
        }
    }
}