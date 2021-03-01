using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlMualim.Models
{
    public class Notes
    {
        public int ID {get; set;}

        [StringLength(128)]
        [Required]
        public string Title {get; set;}
        public string Description {get; set;}
        public int? Surah {get; set;}
        public int? Ruku {get; set;}
        public string Url {get; set;}
        public DateTime DateAdded {get; set;}
        public DateTime LastUpdated {get; set;}
        public List<Topics> Topics {get; set;}
        public List<Tags> Tags {get; set;}

        public string GetSearchString(Surah surah = null)
        {
            var searchString = Title + " " + Description + " ";
            if (surah != null)
                searchString += Surah.ToString() + " " + Ruku + " ";
            if (Topics != null)
                Topics.ForEach(t => searchString += t.Title + " ");
            if (Tags != null)
                Topics.ForEach(t => searchString += t.Title + " ");

            return searchString.ToLower();
        }
    }
}