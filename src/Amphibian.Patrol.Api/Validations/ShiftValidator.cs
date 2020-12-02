using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;

using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Controllers;
using Amphibian.Patrol.Api.Repositories;
using System.Data;
using Amphibian.Patrol.Api.Dtos;

namespace Amphibian.Patrol.Api.Validations
{
    public class ShiftValidator : AbstractValidator<Shift>
    {
        public ShiftValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(25);
            RuleFor(x => x.PatrolId).GreaterThan(0);
            RuleFor(x => x.StartHour).GreaterThanOrEqualTo(0).LessThanOrEqualTo(23);
            RuleFor(x => x.EndHour).GreaterThanOrEqualTo(0).LessThanOrEqualTo(23);
            RuleFor(x => x.StartMinute).GreaterThanOrEqualTo(0).LessThanOrEqualTo(59);
            RuleFor(x => x.EndMinute).GreaterThanOrEqualTo(0).LessThanOrEqualTo(59);
        }
    }
}
