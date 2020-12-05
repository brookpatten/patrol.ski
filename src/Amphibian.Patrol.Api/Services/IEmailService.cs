using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface IEmailService
    {
        //registration/authentication
        Task SendRegistrationEmail(User user);
        Task SendResetEmail(User user, string resetRoute);
        Task SendNewUserWelcomeEmail(User user, string patrolName, string welcomeRoute);
        Task SendExistingUserWelcomeEmail(User user, string patrolName, string welcomeRoute);
        
        //announcements/events
        Task SendOneOffEmail(User from, List<User> users, string patrolName, string subject, string textMessage, string htmlMessage);
        Task SendAnnouncementEmail(User from, List<User> users, string patrolName, string subject, string textMessage, string htmlMessage);
        Task SendEventEmail(User fromUser, List<User> users, Models.Patrol patrol, string name, string location, string textMessage, string htmlMessage, DateTime from, DateTime to);

        //training shifts
        Task SendTraineeSignup(User trainerUser,User traineeUser,Models.Patrol patrol,ScheduledShift shift);
        Task SendTraineeCancel(User trainerUser, User traineeUser, Models.Patrol patrol, ScheduledShift shift);
        Task SendTrainerShiftReleased(User trainerUser, List<User> traineeUsers, Models.Patrol patrol, ScheduledShift shift);
        Task SendTrainerShiftRemoved(User trainerUser, List<User> traineeUsers, Models.Patrol patrol, ScheduledShift shift);

        //training
        Task AssignmentCreated(User assignedTo, Models.Patrol patrol, Assignment assignment, Plan plan);
        Task SignaturesReceived(User trainer, Models.Patrol patrol, User trainee, Assignment assignment, Plan plan, List<Tuple<Section,Skill,Level>> signatures, DateTime when);
        Task AssignmentCompleted(User trainee, Models.Patrol patrol, Assignment assignment, Plan plan);
        Task AssignmentCancelled(User trainee, Models.Patrol patrol, Assignment assignment, Plan plan);

        //scheduling
        Task SendShiftClaimed(User assigned, Models.Patrol patrol, User claimed, ScheduledShift shift);
        Task SendShiftApproved(User assigned, Models.Patrol patrol, User claimed, User approved, ScheduledShift shift);
        Task SendShiftRejected(User assigned, Models.Patrol patrol, User claimed, User rejected, ScheduledShift shift);
        Task SendShiftRemoved(User assigned, Models.Patrol patrol, ScheduledShift shift);
        Task SendShiftCancelled(List<User> assigned, Models.Patrol patrol, ScheduledShift shift);
        Task SendShiftAdded(List<User> assigned, Models.Patrol patrol, ScheduledShift shift);
    }
}
