namespace LearningApp.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        public int? RequirementId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalChapters { get; set; }
    }

    public class Chapter
    {
        public int ChapterId { get; set; }
        public int CourseId { get; set; }
        public int ChapterNumber { get; set; }
        public required string ChapterName { get; set; }
        public string? ChapterDescription { get; set; }
    }

    public class Subtopic
    {
        public int SubtopicId { get; set; }
        public int ChapterId { get; set; }
        public int CourseId { get; set; }
        public int SubtopicNumber { get; set; }
        public required string SubtopicName { get; set; }
        public string? SubtopicDescription { get; set; }
        public int EstimatedDurationMinutes { get; set; }
    }

    public class SubtopicContent
    {
        public int SubtopicId { get; set; }
        public int ChapterId { get; set; }
        public int CourseId { get; set; }
        public int SubtopicNumber { get; set; }
        public required string SubtopicName { get; set; }
        public string? SubtopicDescription { get; set; }
        public string? FullContent { get; set; }
        public int EstimatedDurationMinutes { get; set; }
    }

    public class ProgressUpdate
    {
        public int UserId { get; set; }
        public int SubtopicId { get; set; }
        public bool IsViewed { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class SubtopicProgress
    {
        public int ProgressId { get; set; }
        public int UserId { get; set; }
        public int SubtopicId { get; set; }
        public bool IsViewed { get; set; }
        public bool IsCompleted { get; set; }
        public int ViewCount { get; set; }
        public DateTime LastViewedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class CourseProgress
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        public int TotalSubtopics { get; set; }
        public int CompletedSubtopics { get; set; }
        public double ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
    }
}
