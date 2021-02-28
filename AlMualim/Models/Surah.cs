using System;
using System.ComponentModel.DataAnnotations;

namespace AlMualim.Models
{
    public class Surah
    {
        public int ID {get; set;}
        public string Title {get; set;}
        public string Translation {get; set;}
        public string Slug {get; set;}

        public Surah(string title, string translation)
        {
            Title = title;
            Translation = translation;
            Slug = title.ToLower().Replace("'", "").Replace(" ", "");
        }

        public override string ToString()
        {
            return $"{Title} - ({Translation})";
        }
    }
}