using AutoMapper;
using RealWord.Data.Entities;
using RealWord.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Core.Profiles
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
