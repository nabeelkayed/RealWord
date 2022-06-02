using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealWord.Db.Entities;
using RealWord.Web.Models;

namespace RealWord.Web.Profiles
{
    public class CommentsProfile : Profile
    {
        public CommentsProfile()
        {
            CreateMap<Comment, CommentDto>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.CommentId));
            /*.ForMember(
                  dest => dest.Author,
                  opt => opt.MapFrom(src => Mapper.Map<ProfileDto>(src.User));*/

            /*  .ForMember(dest => dest.destItem, opt => opt.MapFrom(src => Mapper.Map<DestinationSubItem>(src));

                     Mapper.CreateMap<Source, DestinationSubItem>()
                       .ForMember(dest => dest.propertyA, opt => opt.MapFrom(src => src.subItemA.subPropertyA)
                       .ForMember(dest => dest.propertyB, opt => opt.MapFrom(src => src.subItemB.subPropertyB);*/

            /* .ForMember(
                  dest => dest.Author,
                  opt => opt.MapFrom(src => src.User));*/
            CreateMap<CommentForCreationDto, Comment>();
        }
        /*public IMapper nn()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Comment, CommentDto>();

                cfg.CreateMap<User, ProfileDto>()
               .ForMember(
                    dest => dest.Following,
                    opt => opt.MapFrom((src, dest, destMember, context) => src.Followers.Select(s => s.FollowerId).ToList()
                              .Contains((Guid)context.Items["currentUserId"])));
            });
            config.AssertConfigurationIsValid();

            return config.CreateMapper();
        }*/
    }
}