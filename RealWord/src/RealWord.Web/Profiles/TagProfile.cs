using AutoMapper;
using RealWord.Db.Entities;
using RealWord.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Profiles
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<List<Tag>, TagDto>()
                .ForMember(
                    dest => dest.Tags,
                    opt => opt.MapFrom(src => src.Select(a=>a.TagId).ToList()));
        }
    }
}
