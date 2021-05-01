using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlMualim.Models;

namespace AlMualim.Services
{
    public interface IEmailService 
    {
        Task SendEmailAsync(Email email);
        Task SendEmailsAsync(List<Email> emails);
        
        void SendEmail(Email email);
        void SendEmails(List<Email> emails);
    }
}

