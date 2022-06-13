using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealWord.Data.Entities;
using RealWord.Core.Models;

namespace RealWord.Core.Profiles
{
    public class CommentsProfile : Profile
    {
        public CommentsProfile()
        {
            CreateMap<Comment, CommentDto>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.CommentId));

            CreateMap<CommentForCreationDto, Comment>();
        }
    }
}