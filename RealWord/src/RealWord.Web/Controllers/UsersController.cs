﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealWord.Core.Models;
using RealWord.Data.Repositories;
using RealWord.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using RealWord.Core.Auth;
using Microsoft.Net.Http.Headers;
using RealWord.Utils.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using RealWord.Core.Repositories;

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
        private readonly IUserService _IUserService;

        public UsersController(IUserRepository userRepository, IAuthentication authentication,
            IConfiguration config, IMapper mapper, IUserService userService)
        {
            _IUserService = userService ??
               throw new ArgumentNullException(nameof(userService));
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
            var ValidLoginUser = await _IUserService.ValidLoginUserAsync(userLogin);
            if (ValidLoginUser == null)
            {
                return NotFound();
            }

            var userToReturn = _IUserService.LoginUserAsync(ValidLoginUser);
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
            await _IUserRepository.SaveChangesAsync();

            var userToReturn = _mapper.Map<UserDto>(userEntityForCreation);
            userToReturn.Token = await HttpContext.GetTokenAsync("access_token");
            return new ObjectResult(new { user = userToReturn }) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet("user")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userToReturn =await _IUserService.GetCurrentUserAsync();

            userToReturn.Token = await HttpContext.GetTokenAsync("access_token");
            return Ok(new { user = userToReturn });
        }

        [HttpPut("user")]
        public async Task<ActionResult<UserDto>> UpdateUser(UserForUpdateDto userForUpdate)
        {
            var currentUser = await _IUserService.GetCurrentUserAsync();

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
            await _IUserRepository.SaveChangesAsync();

            var userToReturn = _mapper.Map<UserDto>(currentUser);
            userToReturn.Token = await HttpContext.GetTokenAsync("access_token");
            return Ok(new { user = userToReturn });
        }
    }
}