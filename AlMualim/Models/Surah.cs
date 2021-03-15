using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace AlMualim.Models
{
    public class Surah
    {
        public static Dictionary<int, Surah> AllSurah {get; set;}
        public static List<Surah> List() => AllSurah.Values.ToList();

        public int ID {get; set;}
        public string Title {get; set;}
        public string Translation {get; set;}
        public string Arabic {get; set;}
        public string Slug {get; set;}

        public Surah(int id, string title, string translation, string arabic)
        {
            ID = id;
            Title = title;
            Translation = translation;
            Arabic = arabic;
            Slug = title.ToLower().Replace("'", "").Replace(" ", "");
        }

        public override string ToString()
        {
            return $"{Title} ({Translation})";
        }
    }
}