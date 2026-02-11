using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace LearningApp.Controllers
{
    public class LearningController : Controller
    {
        private const string ApiUrl = "http://localhost:5211/api/requirement";

        [HttpGet]
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

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
            Console.WriteLine($"UserId from session: {userId}");
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
            Console.WriteLine($"API Response Status: {response.StatusCode}");
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Home");

            ViewBag.Error = "Failed to save data";
            return View();
        }

    }
}
