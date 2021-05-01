using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlMualim.Models;

namespace AlMualim.Services
{
    public interface IEmailQueue 
    {
        void Enqueue(Notes notes);
        List<Notes> GetNotes();
    }
}

