using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningApp.Models
{
    public class Content
    {
        public int Id { get; set; }
        public int RequirementId { get; set; }
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        public required string CourseJson { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }
}