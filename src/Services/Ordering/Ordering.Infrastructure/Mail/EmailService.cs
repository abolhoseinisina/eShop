using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Mail
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> logger;
        public EmailSettings emailSettings { get; }

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            this.emailSettings = emailSettings.Value;
            this.logger = logger;
        }

        public async Task<bool> SendEmail(Email email)
        {
            var client = new SendGridClient(emailSettings.ApiKey);

            var subject = email.Subject;
            var to = new EmailAddress { Email = email.To };
            var emailBody = email.Body;

            var from = new EmailAddress
            {
                Email = emailSettings.FromAddress,
                Name = emailSettings.FromName
            };

            var sendGridMessage = MailHelper.CreateSingleEmail(from, to, subject, emailBody, emailBody);
            var response = await client.SendEmailAsync(sendGridMessage);

            logger.LogInformation("Sent Email.");

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted || response.StatusCode == System.Net.HttpStatusCode.OK)
                return true;
            logger.LogError("Email sending failed.");
            return false;
        }
    }
}
