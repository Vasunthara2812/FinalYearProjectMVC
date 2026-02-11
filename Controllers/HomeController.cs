using Microsoft.AspNetCore.Mvc;
using LearningApp.Models;
using System.Text.Json;

namespace LearningApp.Controllers
{
    public class HomeController : Controller
    {
        private const string ApiUrl = "http://localhost:5211/api/course";

        public async Task<IActionResult> Index(int? courseId, string search)
        {
            
             if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Login");
            }
            using var client = new HttpClient();

            var apiUrl = ApiUrl;

            // Build query string properly
            var queryParams = new List<string>();
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
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
