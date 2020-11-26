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
    public class RegistrationValidator : AbstractValidator<AuthenticationController.RegistrationRequest>
    {
        public RegistrationValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.firstname).NotEmpty().MaximumLength(256);
            RuleFor(x => x.lastname).NotEmpty().MaximumLength(256);
            RuleFor(x => x.password).NotEmpty();
            RuleFor(x => x.email)
                .EmailAddress().WithMessage("Not a valid Email Address").MaximumLength(512)
                .MustAsync(async (field, token) =>
                {
                    var existing = await userRepository.GetUser(field);
                    return existing == null;
                }).WithMessage("Already in use, please reset your password if you have forgotten it.");
        }
    }
}
