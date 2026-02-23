namespace LearningApp.Models
{
    public class TrainingRequirement
    {
        public int RequirementId { get; set; }   // ✅ ADD THIS

        public int UserId { get; set; }

        public required string Role { get; set; }
        public required string Domain { get; set; }
        public required string Interest { get; set; }
        public required string SkillLevel { get; set; }
        public required string Goal { get; set; }

        public bool CourseGenerated { get; set; }

        public string? CourseName { get; set; }
        public int? CourseId { get; set; }
    }
}