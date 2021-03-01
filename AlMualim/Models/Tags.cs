using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlMualim.Models
{
    public class Tags
    {
        public int ID {get; set;}
        [StringLength(128)]
        [Required]
        public string Title {get; set;}
        public List<Notes> Notes {get; set;}
    }
}