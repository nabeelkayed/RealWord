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
        public IActionResult login(UserLoginDto UserLogin)
        {
            var UserLogedin = _IAuthentication.LoginUser(UserLogin);
            if (UserLogedin == null)
            {
                return Unauthorized();
            }

            var UserToReturn = _mapper.Map<UserDto>(UserLogedin);
            UserToReturn.Token = _IAuthentication.Generate(UserLogedin);

            return Ok(new { user = UserToReturn });
        }
        [AllowAnonymous]
        [HttpPost("users")]
        public ActionResult<UserDto> CreateUser(UserForCreationDto UserForCreation)
        {
            //لازم أعمل فحص اذا كانت الإيميلات متشابهة أو لا
            if (_IUserRepository.UserExists(UserForCreation.Username))
            {
                return NotFound("The user is exist");//لازم الي برجع يكون أفضل من هيك مع ستاتس كود
            }

            var UserEntityForCreation = _mapper.Map<User>(UserForCreation);
            _IUserRepository.CreateUser(UserEntityForCreation);//هل الكريت ترجع والا ما بلزم 
            _IUserRepository.Save();

            var UserToReturn = _mapper.Map<UserDto>(UserEntityForCreation);
            UserToReturn.Token = Request.Headers[HeaderNames.Authorization];

            return Ok(new { user = UserToReturn });
        }

        [HttpGet("user")]
        public ActionResult<UserDto> GetCurrentUser()
        {
            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var CurrentUser = _IUserRepository.GetUser(CurrentUsername);

            var UserToReturn = _mapper.Map<UserDto>(CurrentUser);
            UserToReturn.Token = Request.Headers[HeaderNames.Authorization].ToString();

            return Ok(new { user = UserToReturn });
        }

        [HttpPut("user")]
        public ActionResult<UserDto> UpdateUser(UserForUpdateDto UserForUpdate)
        {
            var CurrentUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var CurrentUser = _IUserRepository.GetUser(CurrentUsername);

            var UserEntityForUpdate = _mapper.Map<User>(UserForUpdate);
            var UpdatedUser = _IUserRepository.UpdateUser(CurrentUser, UserEntityForUpdate);
            _IUserRepository.Save();

            var UserToReturn = _mapper.Map<UserDto>(UpdatedUser);
            UserToReturn.Token = Request.Headers[HeaderNames.Authorization].ToString();

            return Ok(new { user = UserToReturn });
        }
    }
}