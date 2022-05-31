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

            var user = _mapper.Map<UserDto>(userLogedin);

            user.token = _IAuthentication.Generate(userLogedin);

            return Ok(new { user = user });
        }
        [AllowAnonymous]
        [HttpPost("users")]
        public ActionResult<UserDto> CreateUser(UserForCreationDto user)
        {
            var User = _IUserRepository.GetUser(user.username);
            //لازم أعمل فحص اذا كانت الإيميلات متشابهة أو لا
            if (User != null)
            {
                return NotFound("The user is exist");//لازم الي برجع يكون أفضل من هيك مع ستاتس كود
            }

            var UserEntity = _mapper.Map<User>(user);

            _IUserRepository.CreateUser(UserEntity);
            _IUserRepository.Save();
            var x = _mapper.Map<UserDto>(UserEntity);
            x.token = Request.Headers[HeaderNames.Authorization];
            return Ok(new { user = x });
        }

        [HttpGet("user")]
        public ActionResult<UserDto> GetCurrentUser()
        {
            /*if(idClaim != null)
           {
               return Ok($"This is your Id: {idClaim.Value}");
           }*/
            var username = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var CurrentUser = _IUserRepository.GetUser(username);
            var User = _mapper.Map<UserDto>(CurrentUser);
            User.token = Request.Headers[HeaderNames.Authorization].ToString();
            return Ok(new { user = User });
        }

        [HttpPut("user")]
        public ActionResult<UserDto> UpdateUser(UserForUpdateDto user)
        {
            var username = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            // map the entity to a CourseForUpdateDto
            // apply the updated field values to that dto
            // map the CourseForUpdateDto back to an entity
            var user1 = _IUserRepository.GetUser(username);

            //_mapper.Map(user, user1);
            var x =_mapper.Map<User>(user);
            //x.Username = username;
            var xx = _IUserRepository.UpdateUser(user1, x);
            _IUserRepository.Save();

            var ss = _mapper.Map<UserDto>(xx);
            ss.token = Request.Headers[HeaderNames.Authorization].ToString();

            return Ok(new { user = ss });
        }
    }
}