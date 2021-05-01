using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using AlMualim.Services;
using AlMualim.Models;

namespace AlMualim.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task SendEmailsAsync(List<Email> emails) 
        {
            var apiKey = _config.GetValue<string>("SendGridApiKey");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("noreply@almualim.ca", "AlMualim");
            var plainTextContent = "";

            foreach(var email in emails)
            {
                var to = new EmailAddress(email.ToEmail, email.Name);
                var msg = MailHelper.CreateSingleEmail(from, to, email.Subject, plainTextContent, email.Message);
                var responseCode = await client.SendEmailAsync(msg);
            }
        }
        
        public async Task SendEmailAsync(Email email) {await SendEmailsAsync(new List<Email>{email});}
        public void SendEmail(Email email) {SendEmails(new List<Email>{email});}
        public void SendEmails(List<Email> emails) {SendEmailsAsync(emails).Wait();}
    }
}