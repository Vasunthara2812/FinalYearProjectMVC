using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using LearningApp.Models;

namespace LearningApp.Controllers
{
    public class CourseController : Controller
    {
        private readonly IConfiguration _configuration;
        private const string BackendApiUrl = "http://localhost:8000";

        public CourseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ===============================
        // 1. GET - List All Courses (Home Page)
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync($"{BackendApiUrl}/courses/");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Failed to load courses";
                    return View(new List<Course>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var courses = JsonSerializer.Deserialize<List<Course>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(courses ?? new List<Course>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<Course>());
            }
        }

        // ===============================
        // 2. GET - Show Chapters for Course
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Chapters(int courseId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                using var client = new HttpClient();
                
                // Get course details
                var courseResponse = await client.GetAsync($"{BackendApiUrl}/courses/");
                List<Course> courses = new();
                if (courseResponse.IsSuccessStatusCode)
                {
                    var courseJson = await courseResponse.Content.ReadAsStringAsync();
                    courses = JsonSerializer.Deserialize<List<Course>>(courseJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }

                var course = courses.FirstOrDefault(c => c.CourseId == courseId);

                // Get chapters for this course
                var chaptersResponse = await client.GetAsync($"{BackendApiUrl}/courses/{courseId}/chapters/");

                if (!chaptersResponse.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Failed to load chapters";
                    ViewBag.Course = course;
                    return View(new List<Chapter>());
                }

                var chaptersJson = await chaptersResponse.Content.ReadAsStringAsync();
                var chapters = JsonSerializer.Deserialize<List<Chapter>>(chaptersJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                ViewBag.Course = course;
                ViewBag.CourseId = courseId;
                ViewBag.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;

                return View(chapters ?? new List<Chapter>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<Chapter>());
            }
        }

        // ===============================
        // 3. GET - Show Subtopics for Chapter
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Subtopics(int chapterId, int courseId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                using var client = new HttpClient();

                // Get chapter details
                var chaptersResponse = await client.GetAsync($"{BackendApiUrl}/courses/{courseId}/chapters/");
                List<Chapter> chapters = new();
                if (chaptersResponse.IsSuccessStatusCode)
                {
                    var chaptersJson = await chaptersResponse.Content.ReadAsStringAsync();
                    chapters = JsonSerializer.Deserialize<List<Chapter>>(chaptersJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }

                var chapter = chapters.FirstOrDefault(c => c.ChapterId == chapterId);

                // Get subtopics for this chapter
                var subtopicsResponse = await client.GetAsync($"{BackendApiUrl}/chapters/{chapterId}/subtopics/");

                if (!subtopicsResponse.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Failed to load subtopics";
                    ViewBag.Chapter = chapter;
                    return View(new List<Subtopic>());
                }

                var subtopicsJson = await subtopicsResponse.Content.ReadAsStringAsync();
                var subtopics = JsonSerializer.Deserialize<List<Subtopic>>(subtopicsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                ViewBag.Chapter = chapter;
                ViewBag.CourseId = courseId;
                ViewBag.ChapterId = chapterId;
                ViewBag.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;

                return View(subtopics ?? new List<Subtopic>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<Subtopic>());
            }
        }

        // ===============================
        // 4. GET - Get Subtopic Content (AJAX)
        // ===============================
        [HttpGet]
        public async Task<IActionResult> GetSubtopicContent(int subtopicId)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync($"{BackendApiUrl}/subtopics/{subtopicId}/content/");

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { error = "Failed to load content" });
                }

                var json = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<SubtopicContent>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Json(content ?? new SubtopicContent { SubtopicId = subtopicId, SubtopicName = "" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // ===============================
        // 5. POST - Update Progress (AJAX)
        // ===============================
        [HttpPost]
        public async Task<IActionResult> UpdateProgress([FromBody] ProgressUpdate progressData)
        {
            try
            {
                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(progressData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{BackendApiUrl}/progress/update/", content);

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, error = "Failed to update progress" });
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var progress = JsonSerializer.Deserialize<SubtopicProgress>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Json(new { success = true, progress });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // ===============================
        // 6. GET - Get Course Progress (AJAX)
        // ===============================
        [HttpGet]
        public async Task<IActionResult> GetCourseProgress(int userId, int courseId)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync($"{BackendApiUrl}/progress/course/{userId}/{courseId}/");

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { error = "Failed to load progress" });
                }

                var json = await response.Content.ReadAsStringAsync();
                var progress = JsonSerializer.Deserialize<CourseProgress>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Json(progress ?? new CourseProgress { UserId = userId, CourseId = courseId, CourseName = "" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // ===============================
        // 7. POST - Generate Course
        // ===============================
        [HttpPost]
        public async Task<IActionResult> GenerateCourse([FromBody] dynamic courseData)
        {
            try
            {
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(courseData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{BackendApiUrl}/generate-course/", content);

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, error = "Failed to generate course" });
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var course = JsonSerializer.Deserialize<Course>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Json(new { success = true, course });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
