using FluentValidation;
using RealWord.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Validators
{
    public class ArticleForCreationValidator : AbstractValidator<ArticleForCreationDto>
    {
        public ArticleForCreationValidator()
        {
            RuleFor(E => E.Title).NotEmpty();
            RuleFor(E => E.Description).NotEmpty();
            RuleFor(E => E.Body).NotEmpty();  
        }
    }
}