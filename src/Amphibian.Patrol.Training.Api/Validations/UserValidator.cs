using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;

using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Controllers;

namespace Amphibian.Patrol.Training.Api.Validations
{
    public class UserValidator:AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
        }
    }

    public class RegistrationValidator:AbstractValidator<AuthenticationController.RegistrationRequest>
    {
        public RegistrationValidator()
        {
            RuleFor(x => x.First).NotEmpty();
            RuleFor(x => x.Last).NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
