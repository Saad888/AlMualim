using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlMualim.Models
{
    public class Email
    {
        public string ToEmail {get; set;}
        public string Name {get; set;}
        public string Message {get; set;}
        public string Subject {get; set;}

        public Email(string to, string name, string message, string subject)
        {
            ToEmail = to;
            Name = name;
            Message = message;
            Subject = subject;
        }
    }
}