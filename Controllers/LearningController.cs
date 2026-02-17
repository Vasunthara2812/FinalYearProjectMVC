using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using LearningApp.Models;

namespace LearningApp.Controllers
{
    public class LearningController : Controller
    {
        private const string ApiUrl = "http://localhost:5211/api/requirement";

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
        // POST - Save Requirement
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Index(
            string role,
            string domain,
            string interest,
            string skillLevel,
            string goal)
        {
            using var client = new HttpClient();

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var data = new
            {
                UserId = userId,
                Role = role,
                Domain = domain,
                Interest = interest,
                SkillLevel = skillLevel,
                Goal = goal
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details");
            }

            ViewBag.Error = "Failed to save data";
            return View();
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
