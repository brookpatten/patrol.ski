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
    public class PatrolValidator : AbstractValidator<Amphibian.Patrol.Api.Models.Patrol>
    {
        public PatrolValidator(IPatrolRepository patrolRepository)
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Subdomain)
                .MaximumLength(255)
                .MustAsync(async (model, field, token) =>
                {
                    if ((model.EnableAnnouncements || model.EnableEvents) && model.EnablePublicSite)
                    {
                        var existing = await patrolRepository.GetPatrol(field);
                        return existing == null || existing.Id == model.Id;
                    }
                    else
                    {
                        model.EnablePublicSite = false;
                        model.Subdomain = null;
                        return true;
                    }
                }).WithMessage("Subdomain already in use by another patrol");
        }
    }
}
