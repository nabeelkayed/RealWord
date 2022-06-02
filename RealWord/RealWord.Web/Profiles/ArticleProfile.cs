using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealWord.Db.Entities;
using RealWord.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using RealWord.Db.Repositories;

namespace RealWord.Web.Profiles
{
    public class ArticleProfile : Profile
    {
        private readonly IHttpContextAccessor accessor;
        private readonly IUserRepository _IUserRepository;



        public ArticleProfile(/*IUserRepository userRepository, IHttpContextAccessor accessor*/)
        {
            /* _IUserRepository = userRepository ??
             throw new ArgumentNullException(nameof(UserRepository));
             this.accessor = accessor;
             var x = accessor.HttpContext.User.Claims
         .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
             Guid userId = _IUserRepository.GetUser(x).UserId;*/

            CreateMap<Article, ArticleDto>()
                .ForMember(
                    dest => dest.FavoritesCount,
                    opt => opt.MapFrom(src => src.Favorites.Count))
                /*  .ForMember(
                      dest => dest.Author,
                      opt => opt.MapFrom(src => src.User))*/
                .ForMember(
                    dest => dest.TagList,
                    opt => opt.MapFrom(src => src.Tags.Select(s => s.TagId).ToList()))
                .ForMember(
                    dest => dest.Favorited,
                    opt => opt.MapFrom((src, dest, destMember, context) => src.Favorites.Select(s => s.UserId).ToList()
                              .Contains((Guid)context.Items["currentUserId"])));
               /* .ForPath(
                    dest => dest.Author.Following,
                    opt => opt.MapFrom(src => src.User.Followerings.Select(s => s.FolloweingId).ToList()
                    .Contains(src.UserId)));*/
               /////////////////////////////////////////
            /* .ForMember(
                 dest => dest.Author.Following,
                 opt => opt.MapFrom((Article src, ArticleDto dest, bool destMember, ResolutionContext context) => src.User.Followerings.Select(s => s.FolloweingId).ToList()
                 .Contains((Guid)context.Items["currentUserId"])))
             .ReverseMap();*/

            /*  cfg.CreateMap<Address, AddressDto>();
              cfg.CreateMap<UserDto, User>()
                  .ForMember(d => d.Addresses, opt => opt.MapFrom(s => s.Properties.Addresses))
                  .ReverseMap();*/

            // .AfterMap((src, dest, destMember, context) => dest.Author.Following = );
            //  .ForPath(dest => dest.Author.Following, opt => opt.MapFrom(src => src));
            // (ResolutionContext)
            //
            //(ArticleDto)
            //()
            /*  Mapper.CreateMap<ViewModel, Dto>()
      .ForMember(d => d.CreatedBy, opt => 
      opt.ResolveUsing((src, dest, destMember, res) => res.Context.Options.Items["CreatedBy"]));*/
            /* .ForMember(
               dest => dest.Author.Following,
               opt => opt.MapFrom((src, dest, destMember, context) => src.User.Followerings.Select(s => s.FolloweingId).ToList()
               .Contains((Guid)context.Items["currentUserId"])));*/




            /* .ForMember(
                 dest => dest.Author.Following,
                 opt => opt.MapFrom(src => src.User.Followerings.Select(s => s.FolloweingId).ToList()
                 .Contains(src.UserId)))*/

            /*  */
            ///////////////////////////////////////////////
            //User.Favorites.Any(a=>a.UserId==src.Favorites.Contains());
            CreateMap<ArticleForCreationDto, Article>();
            /*   .ForMember(
                  dest => dest.Tags,
                  opt => opt.MapFrom(src => src.tagList));*/
            CreateMap<ArticleForUpdateDto, Article>();
        }
    }
}
