using AutoMapper;
using RealWord.Data.Entities;
using RealWord.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Core.Profiles
{
    public class ProfileProfile : Profile
    {
        public ProfileProfile()
        {
            CreateMap<User, ProfileDto>()
             .ForMember(
                    dest => dest.Following,
                    opt => opt.MapFrom((src, dest, destMember, context) => src.Followers.Select(s => s.FollowerId).ToList()
                              .Contains((Guid)context.Items["currentUserId"])));
        }
    }
}
