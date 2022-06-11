using RealWord.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RealWord.Core.Services;

namespace RealWord.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tags")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _ITagService;

        public TagsController(ITagService tagService)
        {
            _ITagService = tagService ??
                throw new ArgumentNullException(nameof(tagService));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<TagDto>> GetTags()
        {
            var tagsToReturn = await _ITagService.GetTagsAsync();
           
            return Ok(tagsToReturn);
        }

        [AllowAnonymous]
        [HttpOptions]
        public IActionResult TagsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS");

            return Ok();
        }
    }
}
