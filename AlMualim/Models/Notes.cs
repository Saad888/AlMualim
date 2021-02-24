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
        public ICollection<Topics> Topics {get; set;}
    }
}