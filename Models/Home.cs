namespace LearningApp.Models
{
    public class Enrollment
    {
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        public int RequirementId { get; set; }
    }
}
