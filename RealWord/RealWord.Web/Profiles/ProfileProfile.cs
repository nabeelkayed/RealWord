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
            CreateMap<User, ProfileDto>()
             .ForMember(
                    dest => dest.Following,
                    opt => opt.MapFrom((src, dest, destMember, context) => src.Followers.Select(s => s.FollowerId).ToList()
                              .Contains((Guid)context.Items["currentUserId"])));
            /*.ForPath(
                dest => dest.Author.Following,
                opt => opt.MapFrom(src => src.User.Followerings.Select(s => s.FolloweingId).ToList()
                .Contains(src.UserId)));*/

            /*.ForMember(
                dest => dest.following,
                opt => opt.MapFrom(src => src.Followers.Any(u=>u.FollowerId==)); ;*/
        }
    }
}
