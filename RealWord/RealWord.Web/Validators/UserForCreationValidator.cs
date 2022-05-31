using FluentValidation;
using RealWord.Web.Models;
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
            RuleFor(u => u.username).NotEmpty();
            RuleFor(u => u.email).NotEmpty().EmailAddress().WithErrorCode("422"); ;//.WithMessage("");
            RuleFor(u => u.password).NotEmpty()
                          .NotNull()
                          .MinimumLength(8)
                          .MaximumLength(16)
                          .Matches("^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$").WithMessage("regex error");
        }
    }
}