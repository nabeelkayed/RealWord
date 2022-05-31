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
            RuleFor(u => u.Username).NotEmpty();
            RuleFor(u => u.Email).NotEmpty().EmailAddress().WithErrorCode("422"); ;//.WithMessage("");
            RuleFor(u => u.Password).NotEmpty()
                          .NotNull()
                          .MinimumLength(8)
                          .MaximumLength(16)
                          .Matches("^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$").WithMessage("regex error");
        }
    }
}