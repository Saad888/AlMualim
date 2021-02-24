using System;
using System.ComponentModel.DataAnnotations;

namespace AlMualim.Models
{
    public class Surah
    {
        public int ID {get; set;}
        public string Title {get; set;}
        public string Translation {get; set;}
    }
}