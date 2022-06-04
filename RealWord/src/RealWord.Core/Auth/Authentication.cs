using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealWord.Data.Entities;
using RealWord.Data.Repositories;
using RealWord.Core.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RealWord.Core.Auth
{
    public class Authentication : IAuthentication
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _IUserRepository;
        private readonly IHttpContextAccessor _accessor;

        public Authentication(IUserRepository userRepository, IHttpContextAccessor accessor,
            IConfiguration config)
        {
            _IUserRepository = userRepository ??
                throw new ArgumentNullException(nameof(UserRepository));
            _accessor = accessor ??
                throw new ArgumentNullException(nameof(accessor));
            _config = config ??
                throw new ArgumentNullException(nameof(config));
        }

        public async Task<User> LoginUserAsync(User userLogin)
        {
            var user =await _IUserRepository.LoginUserAsync(userLogin);

            return user;
        }
        public async Task<User> GetCurrentUserAsync()
        {
            var currentUsername = _accessor?.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!String.IsNullOrEmpty(currentUsername))
            {
                var currentUser = await _IUserRepository.GetUserAsync(currentUsername);
                return currentUser;

            }

            return null;
        }
        public string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
        };


            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
