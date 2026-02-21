using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using LearningApp.Models;

namespace LearningApp.Controllers
{
    public class LearningController1 : Controller
    {
        private const string ApiUrl = "http://localhost:5211/api/requirement";
        private const string GenerateCourseApiUrl = "http://localhost:8000/generate-course/";

        // ===============================
        // GET - Load Form Page
        // ===============================
        [HttpGet]
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        // ===============================
        // POST - Save Requirement and Generate Course
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Index(
            string role,
            string domain,
            string interest,
            string skillLevel,
            string goal)
        {
            try
            {
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                
                using var client = new HttpClient();
                
                // Step 1: Call external API to generate course
                var courseGenerationData = new
                {
                    role = role,
                    domain = domain,
                    interest = interest,
                    skill_level = skillLevel,
                    goal = goal
                };

                var jsonRequest = JsonSerializer.Serialize(courseGenerationData);
                var contentRequest = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                
                var courseResponse = await client.PostAsync(GenerateCourseApiUrl, contentRequest);
                
                string? courseName = null;
                int? courseId = null;

                if (courseResponse.IsSuccessStatusCode)
                {
                    var courseJsonResponse = await courseResponse.Content.ReadAsStringAsync();
                    var generatedCourse = JsonSerializer.Deserialize<GeneratedCourseResponse>(courseJsonResponse,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    courseName = generatedCourse?.CourseName;
                    courseId = generatedCourse?.CourseId;
                }
                else
                {
                    ViewBag.Warning = "Course generation API failed, but requirement will be saved.";
                }

                // Step 2: Save requirement with course info to your backend API
                var data = new
                {
                    UserId = userId,
                    Role = role,
                    Domain = domain,
                    Interest = interest,
                    SkillLevel = skillLevel,
                    Goal = goal,
                    CourseName = courseName,
                    CourseId = courseId
                };

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var saveResponse = await client.PostAsync(ApiUrl, content);

                if (saveResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details");
                }

                ViewBag.Error = "Failed to save data";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View();
            }
        }

        // ===============================
        // GET - Show All Requirements
        // ===============================
        public async Task<IActionResult> Details()
        {
            using var client = new HttpClient();

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // 🔥 This endpoint must exist in your API
            var response = await client.GetAsync($"{ApiUrl}/user/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<TrainingRequirement>());
            }

            var json = await response.Content.ReadAsStringAsync();

            var requirements = JsonSerializer.Deserialize<List<TrainingRequirement>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return View(requirements);
        }
    }
}
