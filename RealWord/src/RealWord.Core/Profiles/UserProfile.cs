using AutoMapper;
using RealWord.Data.Entities;
using RealWord.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Core.Profiles
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
