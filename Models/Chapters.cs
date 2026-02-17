using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningApp.Models
{
    public class Chapters
    {
         public int chapterId { get; set; }
        public int courseId { get; set; }
        public required string chapterName { get; set; }

    }
}