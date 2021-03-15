using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AlMualim.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlMualim.Models
{
    public class SurahJsonCollection
    {
        public List<SurahJson> chapters {get; set;}

        public Dictionary<int, Surah> ToSurah() 
        {
            return chapters.Select(c => c.ToSurah()).ToDictionary(s => s.ID);
        }
    }

    public class SurahJson
    {
        public int id {get; set;}
        public string name_simple {get; set;}
        public string name_arabic {get; set;}
        public Dictionary<string, string> translated_name {get; set;}

        public Surah ToSurah() => new Surah(id, name_simple, translated_name["name"], name_arabic);
    }
}

