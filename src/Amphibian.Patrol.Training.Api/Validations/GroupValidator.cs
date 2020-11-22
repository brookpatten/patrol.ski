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
    public class GroupValidator : AbstractValidator<Group>
    {
        public GroupValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
            RuleFor(x => x.PatrolId).GreaterThan(0);
        }
    }
}
