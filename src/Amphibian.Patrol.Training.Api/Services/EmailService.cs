using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SendGrid;
using SendGrid.Helpers.Mail;

namespace Amphibian.Patrol.Training.Api.Services
{
    public class EmailService
    {
        private string _apiKey;
        private string _overrideTo;

        public EmailService(string sendGridApiKey,string overrideTo)
        {
            _apiKey = sendGridApiKey;
            _overrideTo = overrideTo;
        }
        public async Task Test(string toEmail,string toName)
        {
            SendGridClient client = new SendGridClient(new SendGridClientOptions()
            {
                ApiKey = _apiKey
            });
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("admin@patrol.training", "Patrol.Training Admin"),
                Subject = "This is a test from the app",
                PlainTextContent = "Hello, Email!",
                HtmlContent = "<strong>Hello, Email!</strong>",
            };
            msg.AddTo(new EmailAddress(GetToEmail(toEmail), toName));
            var response = await client.SendEmailAsync(msg);

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
