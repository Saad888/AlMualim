using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlMualim.Models
{
    public class Email
    {
        public string To {get; set;}
        public string Message {get; set;}

        public Email(string to, string message)
        {
            To = to;
            Message = message;
        }
    }
}