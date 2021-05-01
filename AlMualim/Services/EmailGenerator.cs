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
    public class EmailGenerator : IEmailGenerator
    {
        public Email GenerateSubEmail(Subscriptions sub) {return null;}
        public List<Email> GenerateBroadcast(List<Notes> notes, List<Subscriptions> subs) {return null;}
    }
}