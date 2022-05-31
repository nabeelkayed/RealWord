using FluentValidation;
using RealWord.Web.Models;
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
            RuleFor(E => E.title).MaximumLength(100);
            RuleFor(E => E.description).MaximumLength(100);
            RuleFor(E => E.body).MaximumLength(10000);
        }
    }
}
