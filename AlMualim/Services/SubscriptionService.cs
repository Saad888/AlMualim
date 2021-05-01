using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using AlMualim.Data;
using AlMualim.Models;
using Microsoft.EntityFrameworkCore;

namespace AlMualim.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly AlMualimDbContext _context;
        private readonly IEmailGenerator _generator;
        private readonly IEmailService _service;
        private readonly IConfiguration _configuration;
        private readonly IEmailQueue _queue; 

        private CancellationTokenSource _tokenSource {get; set;}

        #region Constructors
        public SubscriptionService(AlMualimDbContext context, IEmailGenerator generator, IEmailService service, IConfiguration configuration, IEmailQueue queue)
        {
            // Assing services
            _context = context;
            _generator = generator;
            _service = service;
            _configuration = configuration;
            _queue = queue;

            // Generate source
            _tokenSource = new CancellationTokenSource();
        }
        #endregion

        #region Public Methods
        public async Task SubscribeEmail(string name, string email)
        {
            // Check if email is valid  
            if (!IsValidEmail(email))
                throw new Exception("Invalid Email Format");

            // Check if email already exists
            var existing = await _context.Subscriptions.FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());
            if (existing != null)
                throw new Exception("Email Already Exists");

            // Add email to database
            var newSub = new Subscriptions() {Email = email, Name = name};
            await _context.Subscriptions.AddAsync(newSub);
            await _context.SaveChangesAsync();

            // Generate notification email
            var notificationEmail = _generator.GenerateSubEmail(await _context.Subscriptions.FirstOrDefaultAsync(s => s.Email == email));

            // Send Notification email
            await _service.SendEmailAsync(notificationEmail);
        }

        public async Task<string> UnsubscribeEmail(Guid emailId)
        {
            // Check if subscription exists with provided email
            var sub = await _context.Subscriptions.FirstOrDefaultAsync(s => s.ID == emailId);

            // If it does, delete it
            if (sub != null)
            {
                _context.Subscriptions.Remove(sub);
                await _context.SaveChangesAsync();
                return sub.Email;
            }

            return String.Empty;
        }

        public void Broadcast(Notes newNote)
        {
            // Add notes to the concurrent queue
            _queue.Enqueue(newNote);
            var subscribers = _context.Subscriptions.ToList();

            // If a thread is currently active, cancells the task
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
            }

            _tokenSource = new CancellationTokenSource();
            // Generate a new broadcast thread
            Task.Run(() => DelayedBroadcast(_tokenSource.Token, subscribers));
        }
        #endregion

        #region Private Methods
        private bool IsValidEmail(string email)
        {
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }

        private void DelayedBroadcast(CancellationToken token, List<Subscriptions> subscribers)
        {
            // Get delay time
            var delayTime = _configuration.GetValue<int>("SubscriptionEmailDelay");

            // Sleep thread for delay time (cancel if token fired)
            var cancelRequested = token.WaitHandle.WaitOne(delayTime);
            if (cancelRequested)
                return;

            // Get list of notes and subscribers
            var notesList = _queue.GetNotes();
            if (notesList == null || notesList.Count == 0)
                return;

            // Generate email
            var emails = _generator.GenerateBroadcast(notesList, subscribers);

            // Send emails
            _service.SendEmails(emails);
        }
        #endregion
    }
}