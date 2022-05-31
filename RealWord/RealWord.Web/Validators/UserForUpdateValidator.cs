﻿using FluentValidation;
using RealWord.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Validators
{
    public class UserForUpdateValidator : AbstractValidator<UserForUpdateDto>
    {
        public UserForUpdateValidator()
        {
            RuleFor(u => u.email).EmailAddress();
            RuleFor(u => u.bio).MaximumLength(200);
          /*  RuleFor(u => u.password)//.NotEmpty() عشان ممكن ما ينضاف 
                         //.NotNull()
                         .MinimumLength(8)
                         .MaximumLength(16)
                         .Matches("^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$").WithMessage("regex error");*/
        }
    }
}
