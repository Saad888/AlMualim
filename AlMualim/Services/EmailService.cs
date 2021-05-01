using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs.Models;
using AlMualim.Data;
using AlMualim.Models;
using Microsoft.EntityFrameworkCore;

namespace AlMualim.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(Email email) {return;}
        public async Task SendEmailsAsync(List<Email> emails) {return;}
        
        public void SendEmail(Email email) {return;}
        public void SendEmails(List<Email> emails) {return;}
    }
}