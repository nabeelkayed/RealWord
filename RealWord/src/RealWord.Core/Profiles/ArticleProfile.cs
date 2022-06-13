using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealWord.Data.Entities;
using RealWord.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using RealWord.Data.Repositories;
using RealWord.Core.Auth;

namespace RealWord.Core.Profiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticleDto>()
                .ForMember(
                    dest => dest.FavoritesCount,
                    opt => opt.MapFrom(src => src.Favorites.Count))
                .ForMember(
                    dest => dest.TagList,
                    opt => opt.MapFrom(src => src.Tags.Select(s => s.TagId).ToList()))
                .ForMember(
                    dest => dest.Favorited,
                    opt => opt.MapFrom((src, dest, destMember, context) => src.Favorites.Select(s => s.UserId).ToList()
                              .Contains((Guid)context.Items["currentUserId"])))
                .AfterMap(
                    (src, dest, context) =>
                     dest.Author = context.Mapper.Map<ProfileDto>(src.User, a => a.Items["currentUserId"] = context.Items["currentUserId"]));

            CreateMap<ArticleForCreationDto, Article>();
            CreateMap<ArticleForUpdateDto, Article>();
        }
    }
}
