using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LearningApp.Models
{
    // Models for deserializing the API response
    // Models for deserializing the API response
    public class ATSResult
    {
        [JsonPropertyName("filename")]
        public string Filename { get; set; }
        
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }
        
        [JsonPropertyName("overall_score")]
        public int OverallScore { get; set; }
        
        [JsonPropertyName("overall_percentage")]
        public double OverallPercentage { get; set; }
        
        [JsonPropertyName("grade")]
        public string Grade { get; set; }
        
        [JsonPropertyName("categories")]
        public List<Category> Categories { get; set; }
        
        [JsonPropertyName("strengths")]
        public List<string> Strengths { get; set; }
        
        [JsonPropertyName("weaknesses")]
        public List<string> Weaknesses { get; set; }
        
        [JsonPropertyName("recommendations")]
        public List<string> Recommendations { get; set; }
        
        [JsonPropertyName("keyword_analysis")]
        public KeywordAnalysis KeywordAnalysis { get; set; }
        
        [JsonPropertyName("analyzed_at")]
        public string AnalyzedAt { get; set; }
    }

    public class Category
    {
        [JsonPropertyName("category")]
        public string CategoryName { get; set; }
        
        [JsonPropertyName("score")]
        public int Score { get; set; }
        
        [JsonPropertyName("max_score")]
        public int MaxScore { get; set; }
        
        [JsonPropertyName("feedback")]
        public string Feedback { get; set; }
        
        public int Percentage => MaxScore > 0 ? (Score * 100) / MaxScore : 0;
    }

    public class KeywordAnalysis
    {
        [JsonPropertyName("found_keywords")]
        public List<string> FoundKeywords { get; set; }
        
        [JsonPropertyName("missing_keywords")]
        public List<string> MissingKeywords { get; set; }
        
        [JsonPropertyName("keyword_density")]
        public string KeywordDensity { get; set; }
    }
}