using AutoMapper;
using RealWord.Db.Entities;
using RealWord.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Profiles
{
    public class ProfileProfile : Profile
    {
        public ProfileProfile()
        {
            CreateMap<User, ProfileDto>();
                /*.ForMember(
                    dest => dest.following,
                    opt => opt.MapFrom(src => src.Followers.Any(u=>u.FollowerId==)); ;*/
        }
    }
}
