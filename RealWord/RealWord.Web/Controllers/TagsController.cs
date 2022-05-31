using AutoMapper;
using RealWord.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealWord.Db.Repositories;
using RealWord.Db.Entities;
using Microsoft.AspNetCore.Authorization;

namespace RealWord.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tags")]
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _ITagRepository;
        private readonly IMapper _mapper;

        public TagsController(ITagRepository tagRepository,
            IMapper mapper)
        {
            _ITagRepository = tagRepository ??
                throw new ArgumentNullException(nameof(tagRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<TagDto> GetTags()
        {
            var Tags = _ITagRepository.GetTags();
            return Ok(_mapper.Map<TagDto>(Tags));
        }
    }
}
