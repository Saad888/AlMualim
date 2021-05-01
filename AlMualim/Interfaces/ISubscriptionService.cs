using System;
using System.Threading.Tasks;
using AlMualim.Models;

namespace AlMualim.Services
{
    public interface ISubscriptionService 
    {
        Task SubscribeEmail(string name, string email);
        Task<string> UnsubscribeEmail(Guid emailId);
        void Broadcast(Notes newNote);
    }
}
