using AutoMapper;
using Moq;
using RealWord.Core.Services;
using RealWord.Data.Entities;
using RealWord.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RealWord.Tests
{
    public class GetProfileTest
    {
        private readonly IArticleService _IArticleService;

        public GetProfileTest(IArticleService articleService)
        {
            _IArticleService = articleService ??
                throw new ArgumentNullException(nameof(articleService));
        }

        [Fact]
        public async Task Check()
        {
            //var x = new ArticleService();
            var articleExists = await _IArticleService.ArticleExistsAsync("nabeel");

            Assert.True(articleExists);

            /*Mock<ITagRepository> _ITagRepository = new Mock<ITagRepository>();
              Mock<IMapper> _mapper = new Mock<IMapper>();

              var y = new TagService(_ITagRepository.Object, _mapper.Object);

              var x = y.GetTagsAsync();
              var xx = new List<Tag>();
              Assert.Equal((IEnumerable<Tag>)x, xx);*/
        }
    }
}