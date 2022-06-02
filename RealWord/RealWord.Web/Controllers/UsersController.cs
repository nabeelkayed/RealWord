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
        private IConfiguration _config;


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
        public IActionResult login(UserLoginDto userLogin)
        {
            var userLogedin = _IAuthentication.LoginUser(userLogin);
            if (userLogedin == null)
            {
                return Unauthorized();
            }

            var userToReturn = _mapper.Map<UserDto>(userLogedin);
            userToReturn.Token = _IAuthentication.Generate(userLogedin);
            return Ok(new { user = userToReturn });
        }

        [AllowAnonymous]
        [HttpPost("users")]
        public ActionResult<UserDto> CreateUser(UserForCreationDto userForCreation)
        {
            if (_IUserRepository.UserExists(userForCreation.Username))
            {
                return NotFound("The user is exist");
            }

            var userEntityForCreation = _mapper.Map<User>(userForCreation);
            _IUserRepository.CreateUser(userEntityForCreation); 
            _IUserRepository.Save();

            var userToReturn = _mapper.Map<UserDto>(userEntityForCreation);
            userToReturn.Token = Request.Headers[HeaderNames.Authorization];
            return Ok(new { user = userToReturn });
        }

        [HttpGet("user")]
        public ActionResult<UserDto> GetCurrentUser()
        {
            var currentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = _IUserRepository.GetUser(currentUsername);

            var userToReturn = _mapper.Map<UserDto>(currentUser);
            userToReturn.Token = Request.Headers[HeaderNames.Authorization].ToString();
            return Ok(new { user = userToReturn });
        }

        [HttpPut("user")]
        public ActionResult<UserDto> UpdateUser(UserForUpdateDto userForUpdate)
        {
            var currentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = _IUserRepository.GetUser(currentUsername);

            var userEntityForUpdate = _mapper.Map<User>(userForUpdate);
            _IUserRepository.UpdateUser(currentUser, userEntityForUpdate);
            _IUserRepository.Save();
             
            var userToReturn = _mapper.Map<UserDto>(currentUser);
            userToReturn.Token = Request.Headers[HeaderNames.Authorization].ToString();
            return Ok(new { user = userToReturn });
        }
    }
}