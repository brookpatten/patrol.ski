using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SendGrid;
using SendGrid.Helpers.Mail;

using Amphibian.Patrol.Api.Models;
using System.Net;
using Amphibian.Patrol.Api.Extensions;
using Microsoft.Extensions.Logging;
using System.IO;
using Amphibian.Patrol.Configuration;

namespace Amphibian.Patrol.Api.Services
{
    public class SendGridEmailException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Body { get; set; }
    }
    public class EmailService:IEmailService
    {
        private string _apiKey;
        private string _overrideTo;
        private string _fromName;
        private string _fromEmail;
        private string _urlRoot;
        private string _profileRoute;
        private ILogger<EmailService> _logger;
        private string _userFileRelativeUrl;
        private string _userFilePath;
        

        public EmailService(EmailConfiguration emailConfig,AppConfiguration appConfig, ILogger<EmailService> logger)
        {
            _apiKey = emailConfig.SendGridApiKey;
            _overrideTo = emailConfig.SendAllEmailsTo;
            _fromName = emailConfig.FromName;
            _fromEmail = emailConfig.FromEmail;
            _urlRoot = appConfig.RootUrl;
            _profileRoute = emailConfig.ProfileRoute;
            _logger = logger;

            _userFilePath = appConfig.UserFilePath;
            _userFileRelativeUrl = appConfig.UserFileRelativeUrl;
        }
        public async Task SendRegistrationEmail(User user)
        {
            var msg = new SendGridMessage()
            {
                Subject = "Patrol.Ski Registration Complete",
                PlainTextContent = 
                @$"Hello {user.FirstName}, 
Thank you for registering with Patrol.Ski.",
                HtmlContent = @$"Hello {user.FirstName},<br/>Thank you for registering with Patrol.Ski.",
            };

            AddToUnsubscribe(msg);

            var response = await Send(msg,null, user);
        }

        public async Task SendResetEmail(User user,string resetRoute)
        {
            var msg = new SendGridMessage()
            {
                Subject = "Patrol.Ski Password Reset",
                PlainTextContent =
                @$"Hello {user.FirstName}, 
Please follow use link to reset your password {_urlRoot}{resetRoute}",
                HtmlContent = @$"Hello {user.FirstName},<br/>Please follow <a href='{_urlRoot}{resetRoute}'>this</a> link ({_urlRoot}{resetRoute}) to reset your password.",
            };

            //fake user allow email notifications to true for this one, if they asked for a reset we can asume this email is ok
            user.AllowEmailNotifications = true;

            var response = await Send(msg, null, user);
        }

        public async Task SendNewUserWelcomeEmail(User user, string patrolName, string welcomeRoute)
        {
            var msg = new SendGridMessage()
            {
                Subject = $"Welcome to {patrolName}",
                PlainTextContent = $"You have been added to {patrolName}.\n{_urlRoot}{welcomeRoute}",
                HtmlContent = $"You have been added to {patrolName}.<br/>{_urlRoot}{welcomeRoute}",
            };
            AddToUnsubscribe(msg);
            var response = await Send(msg, null, user);
        }
        public async Task SendExistingUserWelcomeEmail(User user, string patrolName, string welcomeRoute)
        {
            var msg = new SendGridMessage()
            {
                Subject = $"Welcome to {patrolName}",
                PlainTextContent = $"You have been added to {patrolName}.\n{_urlRoot}{welcomeRoute}",
                HtmlContent = $"You have been added to {patrolName}.<br/>{_urlRoot}{welcomeRoute}",
            };
            AddToUnsubscribe(msg);
            var response = await Send(msg, null, user);
        }

        private void AttachEmbeddedImages(SendGridMessage message)
        {
            var html = message.HtmlContent;

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);

            var imageNodes = htmlDoc.DocumentNode.Descendants("img");

            foreach (var node in imageNodes)
            {
                if (node.Attributes.Contains("src"))
                {
                    var src = node.Attributes["src"].Value;
                    if (src.StartsWith("data:image"))
                    {
                        continue;
                    }
                    else
                    {
                        Attachment attachment = new Attachment();
                        string fileName = src;
                        if (fileName.Contains("/"))
                        {
                            fileName = fileName.Substring(fileName.LastIndexOf("/") + 1);
                        }


                        if (src.StartsWith("/"))
                        {
                            if (src.StartsWith(_userFileRelativeUrl))
                            {
                                string path = Path.Combine(_userFilePath, fileName);
                                using (var fs = new FileStream(path, FileMode.Open))
                                {
                                    var bytes = new byte[fs.Length];
                                    fs.Read(bytes, 0, (int)fs.Length);
                                    attachment.Content = Convert.ToBase64String(bytes);
                                }
                            }
                            else
                            {
                                //another 
                                _logger.LogWarning("unrecognized local image url", src);
                            }
                        }
                        else if (src.StartsWith("http"))
                        {
                            using (var client = new WebClient())
                            {
                                var bytes = client.DownloadData(src);
                                attachment.Content = Convert.ToBase64String(bytes);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("unknown image src format", src);
                            continue;
                        }

                        attachment.Disposition = "inline";
                        attachment.Filename = fileName;
                        attachment.ContentId = fileName;

                        node.SetAttributeValue("src", $"cid:{attachment.ContentId}");
                        message.AddAttachment(attachment);
                    }
                }
            }

            //set the modified html back o the message to update src's to cids
            message.HtmlContent = htmlDoc.DocumentNode.InnerHtml;
        }

        private async Task<Response> Send(SendGridMessage message,Models.Patrol patrol,params User[] to)
        {
            SendGridClient client = new SendGridClient(new SendGridClientOptions()
            {
                ApiKey = _apiKey
            });
            message.From = new EmailAddress(_fromEmail, patrol!=null ? patrol.Name +" @ "+_fromName : _fromName);

            //remove anybody that doesn't want notifications,dedup the to list
            to = to.Where(x=>x.AllowEmailNotifications).GroupBy(x => x.Email).Select(x => x.First()).ToArray();

            bool hasTo = false;
            if(!string.IsNullOrEmpty(_overrideTo))
            {
                if (to.Any(x => x.AllowEmailNotifications))
                {
                    if (_overrideTo != "nobody")
                    {
                        message.AddTo(new EmailAddress(_overrideTo, "Test User"));
                        hasTo = true;
                    }
                }
            }
            else
            {
                var filteredTo = new List<EmailAddress>();
                foreach(var email in to)
                {
                    if(email.Email.Contains("@"))
                    {
                        var domain = email.Email.Substring(email.Email.IndexOf("@") + 1);
                        if(!_urlRoot.Contains(domain))
                        {
                            filteredTo.Add(email.ToEmailAddress());
                        }
                    }
                }

                if (filteredTo.Count > 0)
                {
                    hasTo = true;
                    //remove any that have the same host as this app, since these are test users
                    if (filteredTo.Count > 1)
                    {
                        message.AddBccs(filteredTo.ToList());
                    }
                    else
                    {
                        message.AddTos(filteredTo.ToList());
                    }
                }
            }

            if (hasTo)
            {
                AttachEmbeddedImages(message);

                var response = await client.SendEmailAsync(message);

                if(response.StatusCode!=System.Net.HttpStatusCode.Accepted)
                {
                    throw new SendGridEmailException()
                    {
                        Body = await response.Body.ReadAsStringAsync(),
                        StatusCode = response.StatusCode
                    };
                }

                return response;
            }
            else
            {
                return null;
            }
        }

        private string GetToEmail(string email)
        {
            if(string.IsNullOrEmpty(_overrideTo))
            {
                return email;
            }
            else
            {
                return _overrideTo;
            }
        }

        public async Task SendOneOffEmail(User from,List<User> users,string patrolName,string subject,string textMessage,string htmlMessage)
        {
            var msg = new SendGridMessage()
            {
                Subject = subject,
                PlainTextContent = $"On Behalf of {from.GetFullName()}, {patrolName}\n\n{textMessage}",
                HtmlContent = $"<em>On Behalf of {from.GetFullName()}, {patrolName}</em><br/><br/>{htmlMessage}",
            };

            var to = users.ToArray();

            var response = await Send(msg,new Models.Patrol() { Name = patrolName }, to);
        }

        public async Task SendAnnouncementEmail(User from, List<User> users, string patrolName, string subject, string textMessage, string htmlMessage)
        {
            var msg = new SendGridMessage()
            {
                Subject = subject,
                PlainTextContent = $"Announcement from {from.GetFullName()}, {patrolName}\n\n{textMessage}",
                HtmlContent = $"<em>Announcement from {from.GetFullName()}, {patrolName}</em><br/><br/>{htmlMessage}",
            };

            var to = users.ToArray();

            var response = await Send(msg, new Models.Patrol() { Name = patrolName }, to);
        }

        private void AddOnBehalfOf(SendGridMessage message,Models.Patrol patrol)
        {
            message.PlainTextContent += $"\n\nOn behalf of {patrol.Name}";
            message.HtmlContent += $"<br/><br/><em>On behalf of {patrol.Name}</em>";

            AddToUnsubscribe(message);
        }

        private void AddToUnsubscribe(SendGridMessage message)
        {
            message.PlainTextContent += $"\n\nTo disable email notifications visit {_urlRoot}{_profileRoute}";
            message.HtmlContent += $"<br/><br/>To disable email notifications visit <a href='{_urlRoot}{_profileRoute}'>{_urlRoot}{_profileRoute}<a>";
        }

        public async Task SendEventEmail(User fromUser, List<User> users, Models.Patrol patrol, string name, string location, string textMessage, string htmlMessage, DateTime from, DateTime to)
        {
            var localFrom = from.UtcToPatrolLocal(patrol);
            var localTo = to.UtcToPatrolLocal(patrol);

            var msg = new SendGridMessage()
            {
                Subject = $"New Event: {name} {location} {localFrom.ToShortDateString()}",
                PlainTextContent = $"{fromUser.GetFullName()} has created a new calendar event.\n\nWhat: {name}\nWhere: {location}\nWhen:{localFrom.ToShortDateString()} {localFrom.ToShortTimeString()} - {localTo.ToShortDateString()} {localTo.ToShortTimeString()}\n\n{textMessage}",
                HtmlContent = $"{fromUser.GetFullName()} has created a new calendar event.<br/><br/>What: {name}<br/>Where: {location}<br/>When: {localFrom.ToShortDateString()} {localFrom.ToShortTimeString()} - {localTo.ToShortDateString()} {localTo.ToShortTimeString()}<br/><br/>{htmlMessage}",
            };

            this.AddOnBehalfOf(msg,patrol);

            var response = await Send(msg,patrol, users.ToArray());
        }

        //training shifts
        public async Task SendTraineeSignup(User trainerUser, User traineeUser, Models.Patrol patrol, ScheduledShift shift)
        {
            var localStart = shift.StartsAt.UtcToPatrolLocal(patrol);
            var localEnd = shift.EndsAt.UtcToPatrolLocal(patrol);

            var msg = new SendGridMessage()
            {
                Subject = $"{traineeUser.GetFullName()} would like to train with you {localStart.ToShortDateString()}",
                PlainTextContent = $"{traineeUser.GetFullName()} would like to train with you during your shift on {localStart.ToShortDateString()} {localStart.ToShortTimeString()} - {localEnd.ToShortTimeString()}",
                HtmlContent = $"{traineeUser.GetFullName()} would like to train with you during your shift on {localStart.ToShortDateString()} {localStart.ToShortTimeString()} - {localEnd.ToShortTimeString()}",
            };

            this.AddOnBehalfOf(msg, patrol);

            var response = await Send(msg, patrol, trainerUser);
        }
        public async Task SendTraineeCancel(User trainerUser, User traineeUser, Models.Patrol patrol, ScheduledShift shift)
        {
            var localStart = shift.StartsAt.UtcToPatrolLocal(patrol);
            var localEnd = shift.EndsAt.UtcToPatrolLocal(patrol);

            var msg = new SendGridMessage()
            {
                Subject = $"{traineeUser.GetFullName()} Cancelled for {localStart.ToShortDateString()}",
                PlainTextContent = $"{traineeUser.GetFullName()} has cancelled their request to train with you during your shift on {localStart.ToShortDateString()} {localStart.ToShortTimeString()} - {localEnd.ToShortTimeString()}",
                HtmlContent = $"{traineeUser.GetFullName()} has cancelled their request to train with you during your shift on {localStart.ToShortDateString()} {localStart.ToShortTimeString()} - {localEnd.ToShortTimeString()}",
            };

            this.AddOnBehalfOf(msg, patrol);

            var response = await Send(msg, patrol, trainerUser);
        }
        public async Task SendTrainerShiftReleased(User trainerUser, List<User> traineeUsers, Models.Patrol patrol, ScheduledShift shift)
        {
            var localStart = shift.StartsAt.UtcToPatrolLocal(patrol);
            var localEnd = shift.EndsAt.UtcToPatrolLocal(patrol);

            var msg = new SendGridMessage()
            {
                Subject = $"Trainer Shift Released {localStart.ToShortDateString()}",
                PlainTextContent = $"{trainerUser.GetFullName()} has released a shift in which you wanted to train with them on {localStart.ToShortDateString()} {localStart.ToShortTimeString()} - {localEnd.ToShortTimeString()}.\n  Please reschedule your training.",
                HtmlContent = $"{trainerUser.GetFullName()} has released a shift in which you wanted to train with them on {localStart.ToShortDateString()} {localStart.ToShortTimeString()} - {localEnd.ToShortTimeString()}.<br/>Please reschedule your training.",
            };

            this.AddOnBehalfOf(msg, patrol);

            var response = await Send(msg, patrol, traineeUsers.ToArray());
        }
        public async Task SendTrainerShiftRemoved(User trainerUser, List<User> traineeUsers, Models.Patrol patrol, ScheduledShift shift)
        {
            var localStart = shift.StartsAt.UtcToPatrolLocal(patrol);
            var localEnd = shift.EndsAt.UtcToPatrolLocal(patrol);

            var msg = new SendGridMessage()
            {
                Subject = $"Trainer Shift Removed {localStart.ToShortDateString()}",
                PlainTextContent = $"{trainerUser.GetFullName()}'s shift in which you wanted to train with them on {localStart.ToShortDateString()} {localStart.ToShortTimeString()} - {localEnd.ToShortTimeString()} has been removed.\n  Please reschedule your training.",
                HtmlContent = $"{trainerUser.GetFullName()}'s shift in which you wanted to train with them on {localStart.ToShortDateString()} {localStart.ToShortTimeString()} - {localEnd.ToShortTimeString()} has been removed.<br/>  Please reschedule your training.",
            };

            this.AddOnBehalfOf(msg, patrol);

            var response = await Send(msg, patrol, traineeUsers.ToArray());
        }

        //training
        public async Task AssignmentCreated(User assignedTo,Models.Patrol patrol, Assignment assignment, Plan plan)
        {
            var localEnd = assignment.DueAt.UtcToPatrolLocal(patrol);

            var msg = new SendGridMessage()
            {
                Subject = $"{plan.Name} Assignment",
                PlainTextContent = $"You have been assigned {plan.Name} {(assignment.DueAt.HasValue ? ("Due "+assignment.DueAt.UtcToPatrolLocal(patrol)?.ToShortDateString()): "")}",
                HtmlContent = $"You have been assigned {plan.Name} {(assignment.DueAt.HasValue ? ("Due " + assignment.DueAt.UtcToPatrolLocal(patrol)?.ToShortDateString()) : "")}",
            };

            this.AddOnBehalfOf(msg, patrol);

            var response = await Send(msg, patrol, assignedTo);
        }
        public async Task SignaturesReceived(User trainer, Models.Patrol patrol, User trainee, Assignment assignment, Plan plan, List<Tuple<Section, Skill, Level>> signatures, DateTime when)
        {
            var localWhen = when.UtcToPatrolLocal(patrol);

            string plainText = $"You've got new signatures on {plan.Name} from {trainer.GetFullName()}.\n{localWhen.ToShortDateString()} {localWhen.ToShortTimeString()}\n";
            string html = $"You've got new signatures on {plan.Name} from {trainer.GetFullName()}.<br/>{localWhen.ToShortDateString()} {localWhen.ToShortTimeString()}<br/><table>";

            foreach(var sig in signatures)
            {
                plainText = plainText + $"\t{sig.Item1.Name}\t{sig.Item2.Name}\t{sig.Item3.Name}\n";
                html = html + $"<tr><td>{sig.Item1.Name}</td><td>{sig.Item2.Name}</td><td>{sig.Item3.Name}</td></tr>";
            }
            plainText = plainText + "\nNice work!";
            html = html + "</table><br/><strong>Nice work!</strong>";

            var msg = new SendGridMessage()
            {
                Subject = $"New Signatures on {plan.Name} from {trainer.GetFullName()}",
                PlainTextContent = plainText,
                HtmlContent = html,
            };

            this.AddOnBehalfOf(msg, patrol);

            var response = await Send(msg, patrol, trainee);
        }
        
        public async Task AssignmentCompleted(User trainee, Models.Patrol patrol, Assignment assignment, Plan plan)
        {
            var localEnd = assignment.DueAt.UtcToPatrolLocal(patrol);

            var msg = new SendGridMessage()
            {
                Subject = $"{plan.Name} Assignment Completed",
                PlainTextContent = $"Congratulations, your training assignment {plan.Name} is now complete.",
                HtmlContent = $"Congratulations, your training assignment {plan.Name} is now complete.",
            };

            this.AddOnBehalfOf(msg, patrol);

            var response = await Send(msg, patrol, trainee);
        }

        public async Task AssignmentCancelled(User trainee, Models.Patrol patrol, Assignment assignment, Plan plan)
        {
            var localEnd = assignment.DueAt.UtcToPatrolLocal(patrol);

            var msg = new SendGridMessage()
            {
                Subject = $"{plan.Name} Assignment Cancelled",
                PlainTextContent = $"Your training assignment {plan.Name} has been cancelled.",
                HtmlContent = $"Your training assignment {plan.Name} has been cancelled.",
            };

            this.AddOnBehalfOf(msg, patrol);

            var response = await Send(msg, patrol, trainee);
        }

        //scheduling
        public async Task SendShiftClaimed(User assigned, Models.Patrol patrol, User claimed, ScheduledShift shift)
        {
            var localStart = shift.StartsAt.UtcToPatrolLocal(patrol);
            var localEnd = shift.EndsAt.UtcToPatrolLocal(patrol);

            var msg = new SendGridMessage()
            {
                Subject = $"Shift on {localStart.ToShortDateString()} Claimed by {claimed.GetFullName()}",
                PlainTextContent = $"The shift you released on {localStart.ToShortDateString()} has been claimed by {claimed.GetFullName()}.\nIt is now pending approval.",
                HtmlContent = $"The shift you released on {localStart.ToShortDateString()} has been claimed by {claimed.GetFullName()}.<br/>It is now pending approval.",
            };

            this.AddOnBehalfOf(msg, patrol);

            var response = await Send(msg, patrol, assigned);
        }
        public async Task SendShiftApproved(User assigned, Models.Patrol patrol, User claimed, User approved, ScheduledShift shift)
        {
            var localStart = shift.StartsAt.UtcToPatrolLocal(patrol);
            var localEnd = shift.EndsAt.UtcToPatrolLocal(patrol);

            if(assigned==null)
            {
                var msg = new SendGridMessage()
                {
                    Subject = $"Shift claim on {localStart.ToShortDateString()} to {claimed.GetFullName()} Approved",
                    PlainTextContent = $"Your shift claim on {localStart.ToShortDateString()} to {claimed.GetFullName()} has been approved",
                    HtmlContent = $"Your shift Claim on {localStart.ToShortDateString()} to {claimed.GetFullName()} has been approved",
                };

                this.AddOnBehalfOf(msg, patrol);

                var response = await Send(msg, patrol, claimed);
            }
            else
            {
                var msg = new SendGridMessage()
                {
                    Subject = $"Shift swap on {localStart.ToShortDateString()} from {assigned.GetFullName()} to {claimed.GetFullName()} Approved",
                    PlainTextContent = $"The shift swap on {localStart.ToShortDateString()} from {assigned.GetFullName()} to {claimed.GetFullName()} has been approved",
                    HtmlContent = $"The shift swap on {localStart.ToShortDateString()} from {assigned.GetFullName()} to {claimed.GetFullName()} has been approved",
                };

                this.AddOnBehalfOf(msg, patrol);

                var response = await Send(msg, patrol, assigned, claimed);
            }
            
        }
        public async Task SendShiftRejected(User assigned, Models.Patrol patrol, User claimed, User rejected, ScheduledShift shift)
        {
            var localStart = shift.StartsAt.UtcToPatrolLocal(patrol);
            var localEnd = shift.EndsAt.UtcToPatrolLocal(patrol);

            if (assigned == null)
            {
                var msg = new SendGridMessage()
                {
                    Subject = $"Shift claim on {localStart.ToShortDateString()} to {claimed.GetFullName()} Rejected",
                    PlainTextContent = $"Your shift claim on {localStart.ToShortDateString()} to {claimed.GetFullName()} has been rejected",
                    HtmlContent = $"Your shift Claim on {localStart.ToShortDateString()} to {claimed.GetFullName()} has been rejected",
                };

                this.AddOnBehalfOf(msg, patrol);

                var response = await Send(msg, patrol, claimed);
            }
            else
            {
                var msg = new SendGridMessage()
                {
                    Subject = $"Shift swap on {localStart.ToShortDateString()} from {assigned.GetFullName()} to {claimed.GetFullName()} Rejected",
                    PlainTextContent = $"The shift swap on {localStart.ToShortDateString()} from {assigned.GetFullName()} to {claimed.GetFullName()} has been rejected",
                    HtmlContent = $"The shift swap on {localStart.ToShortDateString()} from {assigned.GetFullName()} to {claimed.GetFullName()} has been rejected",
                };

                this.AddOnBehalfOf(msg, patrol);

                var response = await Send(msg, patrol, assigned, claimed);
            }
        }
        public async Task SendShiftRemoved(User assigned, Models.Patrol patrol, ScheduledShift shift)
        {
            var localStart = shift.StartsAt.UtcToPatrolLocal(patrol);
            var localEnd = shift.EndsAt.UtcToPatrolLocal(patrol);

            var msg = new SendGridMessage()
            {
                Subject = $"Shift on {localStart.ToShortDateString()} removed.",
                PlainTextContent = $"Your shift on {localStart.ToShortDateString()} has been removed.",
                HtmlContent = $"Your shift on {localStart.ToShortDateString()} has been removed.",
            };

            this.AddOnBehalfOf(msg, patrol);

            var response = await Send(msg, patrol, assigned);
        }
        public async Task SendShiftCancelled(List<User> assigned, Models.Patrol patrol, ScheduledShift shift)
        {
            var localStart = shift.StartsAt.UtcToPatrolLocal(patrol);
            var localEnd = shift.EndsAt.UtcToPatrolLocal(patrol);

            var msg = new SendGridMessage()
            {
                Subject = $"Shift on {localStart.ToShortDateString()} cancelled.",
                PlainTextContent = $"Your shift on {localStart.ToShortDateString()} has been cancelled.",
                HtmlContent = $"Your shift on {localStart.ToShortDateString()} has been cancelled.",
            };

            this.AddOnBehalfOf(msg, patrol);

            var response = await Send(msg, patrol, assigned.ToArray());
        }
        public async Task SendShiftAdded(List<User> assigned, Models.Patrol patrol, ScheduledShift shift)
        {
            var localStart = shift.StartsAt.UtcToPatrolLocal(patrol);
            var localEnd = shift.EndsAt.UtcToPatrolLocal(patrol);

            var msg = new SendGridMessage()
            {
                Subject = $"Shift created on {localStart.ToShortDateString()}",
                PlainTextContent = $"You have a new shift on {localStart.ToShortDateString()}, {localStart.ToShortTimeString()} - {localEnd.ToShortTimeString()}.",
                HtmlContent = $"You have a new shift on {localStart.ToShortDateString()}, {localStart.ToShortTimeString()} - {localEnd.ToShortTimeString()}.",
            };

            this.AddOnBehalfOf(msg, patrol);

            var response = await Send(msg, patrol, assigned.ToArray());
        }

    }
}
