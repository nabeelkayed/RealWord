using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealWord.Db.Entities;
using RealWord.Db.Repositories;
using RealWord.Web.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RealWord.Web.Helpers
{
    public class Authentication : ControllerBase, IAuthentication
    {
        private IConfiguration _config;
        private readonly IUserRepository _IUserRepository;
        private readonly IMapper _mapper;


        public Authentication(IUserRepository userRepository,
            IConfiguration config, IMapper mapper)
        {
            _IUserRepository = userRepository ??
                throw new ArgumentNullException(nameof(UserRepository));
            _config = config ??
                throw new ArgumentNullException(nameof(config));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }
     
        public User LoginUser(UserLoginDto userLogin)
        {
            var user = _IUserRepository.LoginUser(_mapper.Map<User>(userLogin));

            return user;
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
