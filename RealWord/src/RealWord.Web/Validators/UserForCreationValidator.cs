using FluentValidation;
using RealWord.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Validators
{
    public class UserForCreationValidator : AbstractValidator<UserForCreationDto>
    {
        public UserForCreationValidator()
        {
            RuleFor(u => u.Username).NotEmpty()
                                    .Matches("^[a-zA-Z][a-zA-Z0-9._-]{0,21}([-.][^_]|[^-.]{2})$");
            RuleFor(u => u.Email).NotEmpty()
                                 .EmailAddress();
            RuleFor(u => u.Password).NotEmpty()
                                    .MinimumLength(8)
                                    .MaximumLength(16)
                                    .Matches("^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$")
                                    .WithMessage("regex error");
        }
    }
}