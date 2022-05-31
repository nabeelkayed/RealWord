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
                    opt => opt.MapFrom(src => src.Favorites.Count));
              /*  .ForMember(
                    dest => dest.favorited,
                    opt => opt.MapFrom(src => src.User.Favorites.Any(a=>a.UserId==src.Favorites.Contains());*/
            CreateMap<ArticleForCreationDto, Article>();
            CreateMap<ArticleForUpdateDto, Article>();
        }
    }
}
