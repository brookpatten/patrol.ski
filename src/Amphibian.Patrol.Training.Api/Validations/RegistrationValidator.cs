using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;

using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Controllers;
using Amphibian.Patrol.Training.Api.Repositories;
using System.Data;

namespace Amphibian.Patrol.Training.Api.Validations
{
    public class RegistrationValidator : AbstractValidator<AuthenticationController.RegistrationRequest>
    {
        public RegistrationValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.First).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Last).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Not a valid Email Address").MaximumLength(512)
                .MustAsync(async (field, token) =>
                {
                    var existing = await userRepository.GetUser(field);
                    return existing == null;
                }).WithMessage("Already in use, please reset your password if you have forgotten it.");
        }
    }
}
