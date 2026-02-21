using Microsoft.AspNetCore.Mvc;
using LearningApp.Models;
using System.Text.Json;

namespace LearningApp.Controllers
{
    public class HomeController : Controller
    {
        private const string ApiUrl = "http://localhost:5211/api/course";
        private const string RequirementApiUrl = "http://localhost:5211/api/requirement";

        public async Task<IActionResult> Index(int? courseId, string search)
        {
            
             if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Login");
            }
            using var client = new HttpClient();
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Fetch Generated Courses from Requirements API
            try
            {
                var requirementResponse = await client.GetAsync($"{RequirementApiUrl}/user/{userId}");
                if (requirementResponse.IsSuccessStatusCode)
                {
                    var json = await requirementResponse.Content.ReadAsStringAsync();
                    var requirements = JsonSerializer.Deserialize<List<TrainingRequirement>>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    // Filter only those with generated courses
                    var generatedCourses = requirements?
                        .Where(r => !string.IsNullOrEmpty(r.CourseName))
                        .ToList() ?? new List<TrainingRequirement>();
                    
                    ViewBag.GeneratedCourses = generatedCourses;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching generated courses: {ex.Message}");
                ViewBag.GeneratedCourses = new List<TrainingRequirement>();
            }

            // Fetch Enrollments
            var apiUrl = ApiUrl;

            // Build query string properly
            var queryParams = new List<string>();
            if(userId > 0)
                queryParams.Add($"userId={userId}");
            if (!string.IsNullOrWhiteSpace(search))
                queryParams.Add($"search={Uri.EscapeDataString(search)}");

            if (queryParams.Any())
                apiUrl += "?" + string.Join("&", queryParams);

            var response = await client.GetAsync(apiUrl);

            var list = new List<Enrollment>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                list = JsonSerializer.Deserialize<List<Enrollment>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<Enrollment>();

                // Populate dropdown BEFORE filtering
                ViewBag.CourseIds = list
                    .Select(e => e.CourseId)
                    .Distinct()
                    .OrderBy(id => id)
                    .ToList();

                // Apply courseId filter locally
                if (courseId.HasValue)
                {
                    list = list.Where(e => e.CourseId == courseId.Value).ToList();
                }
            }

            ViewBag.CourseId = courseId;
            ViewBag.Search = search;

            return View(list);
        }



        
    }
}
