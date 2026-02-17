using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningApp.Models
{
    public class Subtopics
    {
        public int subtopicId { get; set; }
        public int courseId { get; set; }

        public int chapterId { get; set; }
        public required string subtopicName { get; set; }
        
    }
}