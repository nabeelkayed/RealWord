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
                    opt => opt.MapFrom(src => src.CommentId))
                .ForMember(
                    dest => dest.Author,
                    opt => opt.MapFrom(src => src.User));
            CreateMap<CommentForCreationDto, Comment>();
        }
    }
}