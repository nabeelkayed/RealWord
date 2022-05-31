using AutoMapper;
using RealWord.Db.Entities;
using RealWord.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserLoginDto, User>();
            CreateMap<UserForCreationDto, User>();
            CreateMap<UserForUpdateDto, User>();
        }
    }
}
