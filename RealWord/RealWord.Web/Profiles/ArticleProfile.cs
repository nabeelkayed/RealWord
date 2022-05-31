using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealWord.Db.Entities;
using RealWord.Web.Models;
namespace RealWord.Web.Profiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticleDto>()
                .ForMember(
                    dest => dest.favoritesCount,
                    opt => opt.MapFrom(src => src.Favorites.Count)) 
                .ForMember(
                    dest => dest.tagList,
                    opt => opt.MapFrom(src => new TagDto { tags= src.Tags.Select(s => s.TagId).ToList() }));
          /*   .ForMember(////need test
                    dest => dest.author, 
                    opt => opt.MapFrom(src => src.User))*/
            /*  .ForMember(
                  dest => dest.favorited,
                  opt => opt.MapFrom(src => src.User.Favorites.Any(a=>a.UserId==src.Favorites.Contains());*/
            CreateMap<ArticleForCreationDto, Article>();
            /*   .ForMember(
                  dest => dest.Tags,
                  opt => opt.MapFrom(src => src.tagList));*/
            CreateMap<ArticleForUpdateDto, Article>();
        }
    }
}
