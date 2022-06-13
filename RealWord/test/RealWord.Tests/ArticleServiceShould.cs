using AutoMapper;
using Moq;
using RealWord.Core.Services;
using RealWord.Data.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RealWord.Tests 
{
    public class ArticleServiceShould
    {
        [Fact]
        public async Task CheckIfArticleExists()
        {
            //Arrange
            var articleRepository = new Mock<IArticleRepository>();
            var userService = new Mock<IUserService>();
            var tagService = new Mock<ITagService>();
            var mapper = new Mock<IMapper>();

            articleRepository.Setup(x => x.ArticleExistsAsync("how-to-train-your-dragonnabeel-4CDA241617986DA53DBAA07F17FBEC4227290A86")).Returns(Task.FromResult(true));
            var articleService = new ArticleService(articleRepository.Object, userService.Object,
                tagService.Object, mapper.Object);

            //Act 
            var articleExists = await articleService.ArticleExistsAsync("how-to-train-your-dragonnabeel-4CDA241617986DA53DBAA07F17FBEC4227290A86");

            //Assert
            Assert.True(articleExists);
        }
    }
}