using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningApp.Models
{
    public class Result
    {
        public int ResultId { get; set; }
        public int CourseId { get; set; }
        public int ChapterId { get; set; }
        public int SubtopicId { get; set; }
        public string ResultText { get; set; }
    }


}