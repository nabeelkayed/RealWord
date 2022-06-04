﻿using AutoMapper;
using RealWord.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealWord.Data.Repositories;
using RealWord.Data.Entities;
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
        public async Task<ActionResult<TagDto>> GetTags()
        {
            var tags = await _ITagRepository.GetTagsAsync();
            var tagsToReturn = _mapper.Map<TagDto>(tags);
            return Ok(tagsToReturn);
        }
    }
}
