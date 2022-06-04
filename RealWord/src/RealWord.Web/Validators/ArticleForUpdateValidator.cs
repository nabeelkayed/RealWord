using FluentValidation;
using RealWord.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Validators
{
    public class ArticleForUpdateValidator : AbstractValidator<ArticleForUpdateDto>
    {
        public ArticleForUpdateValidator()
        {
            RuleFor(E => E.Title).MaximumLength(100);
            RuleFor(E => E.Description).MaximumLength(100);
            RuleFor(E => E.Body).MaximumLength(10000);
        }
    }
}
