using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlMualim.Models
{
    public class Topics
    {
        public int ID {get; set;}
        [StringLength(128)]
        [Required]
        public string Title {get; set;}
        public ICollection<Notes> Notes {get; set;}
    }
}