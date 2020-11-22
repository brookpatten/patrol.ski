using Amphibian.Patrol.Training.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Services
{
    public interface IEmailService
    {
        Task SendRegistrationEmail(User user);
        Task SendResetEmail(User user, string resetRoute);
        Task SendNewUserWelcomeEmail(User user, string patrolName, string welcomeRoute);
        Task SendExistingUserWelcomeEmail(User user, string patrolName, string welcomeRoute);
    }
}
