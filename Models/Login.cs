using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningApp.Models
{
    public class Login
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        
    }
}