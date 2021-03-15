using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlMualim.Models
{
    public class Stories
    {
        public int ID {get; set;}
        [StringLength(128)]
        [Required]
        public string Prophet {get; set;}
        public List<Notes> Notes {get; set;}
        public int? Order {get; set;}
    }
}