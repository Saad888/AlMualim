using System;
using System.Collections.Generic;
using AlMualim.Models;

namespace AlMualim.Services
{
    public interface IEmailGenerator 
    {
        Email GenerateSubEmail(Subscriptions sub);
        List<Email> GenerateBroadcast(List<Notes> notes, List<Subscriptions> subs);
    }
}


