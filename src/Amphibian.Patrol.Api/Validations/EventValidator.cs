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
    public class EventValidator : AbstractValidator<Event>
    {
        public EventValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(25);
            RuleFor(x => x.PatrolId).GreaterThan(0);
        }
    }
}
