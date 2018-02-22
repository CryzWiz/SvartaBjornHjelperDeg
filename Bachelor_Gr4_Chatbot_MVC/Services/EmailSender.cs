using Bachelor_Gr4_Chatbot_MVC.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // This class is based upon https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<EmailSenderOptions> options)
        {
            Options = options.Value;
        }

        public EmailSenderOptions Options { get; set; }


        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.APIKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("Test@test.test", "Gr4 - ChatBot"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);

        }
    }
}
