﻿using RealWord.Data.Entities;
using RealWord.Utils.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using RealWord.Utils.Utils;
using System.Threading.Tasks;
using RealWord.Core.Models;
using RealWord.Data.Repositories;
using AutoMapper;

namespace RealWord.Core.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _IArticleRepository;
        private readonly IUserService _IUserService;
        private readonly ITagService _ITagService;
        private readonly IMapper _mapper;

        public ArticleService(IArticleRepository articleRepository, IUserService userService,
         ITagService tagService, IMapper mapper)
        {

            _IArticleRepository = articleRepository ??
                throw new ArgumentNullException(nameof(articleRepository));
            _IUserService = userService ??
                throw new ArgumentNullException(nameof(userService));
            _ITagService = tagService ??
                throw new ArgumentNullException(nameof(tagService));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> ArticleExistsAsync(string slug)
        {
            var articleExists = await _IArticleRepository.ArticleExistsAsync(slug);
            return articleExists;
        }
        public async Task<bool> IsAuthorized(string slug)
        {
            var article = await _IArticleRepository.GetArticleAsync(slug);
            var currentUserId = await _IUserService.GetCurrentUserIdAsync();

            var isAuthorized = currentUserId == article.UserId;
            return isAuthorized;
        }
        public async Task<IEnumerable<ArticleDto>> GetArticlesAsync(ArticlesParameters articlesParameters)
        {
            var articles = await _IArticleRepository.GetArticlesAsync(articlesParameters);
            if (articles == null)
            {
                return null;
            }

            var articlesToReturn = new List<ArticleDto>();
            var currentUserId = await _IUserService.GetCurrentUserIdAsync();

            foreach (var article in articles)
            {
                var articleDto = MapArticle(article, currentUserId);
                articlesToReturn.Add(articleDto);
            }

            return articlesToReturn;
        }
        public async Task<IEnumerable<ArticleDto>> FeedArticleAsync(FeedArticlesParameters feedArticlesParameters)
        {
            var currentUserId = await _IUserService.GetCurrentUserIdAsync();

            var articles = await _IArticleRepository.GetFeedArticlesAsync(currentUserId, feedArticlesParameters);
            if (!articles.Any())
            {
                return null;
            }

            var articlesToReturn = new List<ArticleDto>();
            foreach (var article in articles)
            {
                var articleDto = MapArticle(article, currentUserId);
                articlesToReturn.Add(articleDto);
            }

            return articlesToReturn;
        }
        public async Task<ArticleDto> GetArticleAsync(string slug)
        {
            var article = await _IArticleRepository.GetArticleAsync(slug);
            if (article == null)
            {
                return null;
            }

            var articleToReturn = MapArticle(article, Guid.Empty);
            return articleToReturn;
        }
        public async Task<Guid> GetArticleIdAsync(string slug)
        {
            var article = await _IArticleRepository.GetArticleAsync(slug);

            var articleId = article?.ArticleId ?? Guid.Empty;
            return articleId;
        }
        public async Task<ArticleDto> CreateArticleAsync(ArticleForCreationDto articleForCreation)
        {
            var articleEntityForCreation = _mapper.Map<Article>(articleForCreation);

            articleEntityForCreation.UserId = Guid.NewGuid();
            articleEntityForCreation.Slug = articleEntityForCreation.Slug.GenerateSlug(articleEntityForCreation.Title, articleEntityForCreation.UserId);

            var currentUserId = await _IUserService.GetCurrentUserIdAsync();
            articleEntityForCreation.UserId = currentUserId;

            var timeStamp = DateTime.Now;
            articleEntityForCreation.CreatedAt = timeStamp;
            articleEntityForCreation.UpdatedAt = timeStamp;

            await _IArticleRepository.CreateArticleAsync(articleEntityForCreation);
            await _IArticleRepository.SaveChangesAsync();

            if (articleForCreation.TagList != null && articleForCreation.TagList.Any())
            {
                await _ITagService.CreateTags(articleForCreation.TagList, articleEntityForCreation.ArticleId);
            }

            var createdArticleToReturn = MapArticle(articleEntityForCreation, currentUserId);
            return createdArticleToReturn;
        }
        public async Task<ArticleDto> UpdateArticleAsync(string slug, ArticleForUpdateDto articleForUpdate)
        {
            var UpdatedArticle = await _IArticleRepository.GetArticleAsync(slug);
            var currentUserId = await _IUserService.GetCurrentUserIdAsync();

            var articleEntityForUpdate = _mapper.Map<Article>(articleForUpdate);

            if (!string.IsNullOrWhiteSpace(articleForUpdate.Title))
            {
                UpdatedArticle.Title = articleForUpdate.Title;
                UpdatedArticle.Slug.GenerateSlug(articleForUpdate.Title, UpdatedArticle.ArticleId);
            }
            if (!string.IsNullOrWhiteSpace(articleForUpdate.Description))
            {
                UpdatedArticle.Description = articleForUpdate.Description;
            }
            if (!string.IsNullOrWhiteSpace(articleForUpdate.Body))
            {
                UpdatedArticle.Body = articleForUpdate.Body;
            }

            UpdatedArticle.UpdatedAt = DateTime.Now;

            _IArticleRepository.UpdateArticle(UpdatedArticle, articleEntityForUpdate);
            await _IArticleRepository.SaveChangesAsync();
            
            var UpdatedArticleToReturn = MapArticle(UpdatedArticle, currentUserId);
            return UpdatedArticleToReturn;
        }

        public async Task DeleteArticleAsync(string slug)
        {
            var article = await _IArticleRepository.GetArticleAsync(slug);

            _IArticleRepository.DeleteArticle(article);
            await _IArticleRepository.SaveChangesAsync();
        }
        public async Task<ArticleDto> FavoriteArticleAsync(string slug)
        {
            var article = await _IArticleRepository.GetArticleAsync(slug);
            if (article == null)
            {
                return null;
            }

            var currentUserId = await _IUserService.GetCurrentUserIdAsync();

            var isFavorited = await _IArticleRepository.IsFavoritedAsync(currentUserId, article.ArticleId);
            if (isFavorited)
            {
                return null;
            }

            await _IArticleRepository.FavoriteArticleAsync(currentUserId, article.ArticleId);
            await _IArticleRepository.SaveChangesAsync();

            var favoritedaArticleToReturn = MapArticle(article, currentUserId);
            return favoritedaArticleToReturn;
        }
        public async Task<ArticleDto> UnFavoriteArticleAsync(string slug)
        {
            var article = await _IArticleRepository.GetArticleAsync(slug);
            if (article == null)
            {
                return null;
            }

            var currentUserId = await _IUserService.GetCurrentUserIdAsync();

            var isFavorited = await _IArticleRepository.IsFavoritedAsync(currentUserId, article.ArticleId);
            if (!isFavorited)
            {
                return null;
            }

            _IArticleRepository.UnfavoriteArticle(currentUserId, article.ArticleId);
            await _IArticleRepository.SaveChangesAsync();

            var unfavoritedaArticleToReturn = MapArticle(article, currentUserId);
            return unfavoritedaArticleToReturn;
        }
        private ArticleDto MapArticle(Article article, Guid currentUserId)
        {
            var articleToReturn = _mapper.Map<ArticleDto>(article, a => a.Items["currentUserId"] = currentUserId);
            var profileDto = _mapper.Map<ProfileDto>(article.User, a => a.Items["currentUserId"] = currentUserId);
            articleToReturn.Author = profileDto;

            return articleToReturn;
        }
    }
}
