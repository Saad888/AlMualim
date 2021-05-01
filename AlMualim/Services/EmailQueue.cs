using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using AlMualim.Models;

namespace AlMualim.Services
{
    public class EmailQueue : IEmailQueue 
    {
        private ConcurrentQueue<Notes> queue = new ConcurrentQueue<Notes>();

        public void Enqueue(Notes notes) {queue.Enqueue(notes);}

        public List<Notes> GetNotes()
        {
            var list = queue.ToList();
            queue.Clear();
            return list;
        }
    }
}

