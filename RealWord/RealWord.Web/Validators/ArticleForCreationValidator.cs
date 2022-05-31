using FluentValidation;
using RealWord.Web.Models;
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
            RuleFor(E => E.title).NotEmpty();
            RuleFor(E => E.description).NotEmpty();
            RuleFor(E => E.body).NotEmpty();  
        }
    }
}