using AutoMapper;
using System;
using System.Linq;
using RealWord.Data.Entities;
using RealWord.Core.Models;


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
                              .Contains((Guid)context.Items["currentUserId"])));

            CreateMap<ArticleForCreationDto, Article>();
            CreateMap<ArticleForUpdateDto, Article>();
        }
    }
}
