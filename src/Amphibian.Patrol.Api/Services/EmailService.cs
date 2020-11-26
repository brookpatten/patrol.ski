using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SendGrid;
using SendGrid.Helpers.Mail;

using Amphibian.Patrol.Api.Models;

namespace Amphibian.Patrol.Api.Services
{
    public class EmailService:IEmailService
    {
        private string _apiKey;
        private string _overrideTo;
        private string _fromName;
        private string _fromEmail;
        private string _urlRoot;

        public EmailService(string sendGridApiKey,string overrideTo,string fromName,string fromEmail, string urlRoot)
        {
            _apiKey = sendGridApiKey;
            _overrideTo = overrideTo;
            _fromName = fromName;
            _fromEmail = fromEmail;
            _urlRoot = urlRoot;
        }
        public async Task SendRegistrationEmail(User user)
        {
            var msg = new SendGridMessage()
            {
                Subject = "Patrol.Ski Registration Complete",
                PlainTextContent = 
                @$"Hello {user.FirstName}, 
Thank you for registering with Patrol.Training.",
                HtmlContent = @$"Hello {user.FirstName},<br/>Thank you for registering with Patrol.Training.",
            };
            var response = await Send(msg, new EmailAddress(user.Email, user.GetFullName()));
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
            var response = await Send(msg, new EmailAddress(user.Email, user.GetFullName()));
        }

        public async Task SendNewUserWelcomeEmail(User user, string patrolName, string welcomeRoute)
        {

        }
        public async Task SendExistingUserWelcomeEmail(User user, string patrolName, string welcomeRoute)
        {

        }

        private async Task<Response> Send(SendGridMessage message,params EmailAddress[] to)
        {
            SendGridClient client = new SendGridClient(new SendGridClientOptions()
            {
                ApiKey = _apiKey
            });
            message.From = new EmailAddress(_fromEmail, _fromName);

            if(!string.IsNullOrEmpty(_overrideTo))
            {
                message.AddTo(new EmailAddress(_overrideTo, "Test User"));
            }
            else
            {
                message.AddTos(to.ToList());
            }

            var response = await client.SendEmailAsync(message);
            return response;
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
    }
}
