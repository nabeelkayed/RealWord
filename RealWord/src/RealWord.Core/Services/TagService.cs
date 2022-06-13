using RealWord.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RealWord.Data.Repositories;

namespace RealWord.Core.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _ITagRepository;
        private readonly IMapper _mapper;

        public TagService(ITagRepository tagRepository,
            IMapper mapper)
        {
            _ITagRepository = tagRepository ??
                throw new ArgumentNullException(nameof(tagRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<TagDto> GetTagsAsync()
        {
            var tags = await _ITagRepository.GetTagsAsync();

            var tagsToReturn = _mapper.Map<TagDto>(tags);
            return tagsToReturn;
        }
        public async Task CreateTags(List<string> tagList, Guid articleId)
        {
            var tags = await _ITagRepository.GetTagsListAsync();
            var newtags = tagList.Except(tags).ToList();
            if (newtags.Any())
            {
                await _ITagRepository.CreateTagsAsync(newtags, articleId);
            }
            await _ITagRepository.CreateArticleTagsAsync(tagList, articleId);
            await _ITagRepository.SaveChangesAsync();
        }
    }
}