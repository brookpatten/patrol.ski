using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;

using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Controllers;
using Amphibian.Patrol.Training.Api.Repositories;
using System.Data;
using Amphibian.Patrol.Training.Api.Dtos;

namespace Amphibian.Patrol.Training.Api.Validations
{
    public class PatrolUserValidator : AbstractValidator<PatrolUserDto>
    {
        public PatrolUserValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(256);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Not a valid Email Address").MaximumLength(512);
        }
    }
}
