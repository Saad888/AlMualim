using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AlMualim.Models;

namespace AlMualim.Services
{
    public class SearchService
    {


        public List<Notes> GetFilteredNotes(List<Notes> notes, List<Surah> surah, int? surahFilter, int? rukuFilter, int? topicFilter, string searchFilter)
        {
            return null;
        }
    }
}