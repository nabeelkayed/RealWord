using FluentValidation;
using RealWord.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Validators 
{
    public class CommentForCreationValidator : AbstractValidator<CommentForCreationDto>
    {
        public CommentForCreationValidator()
        {
            RuleFor(c => c.Body).NotEmpty();
        }
    }
}
