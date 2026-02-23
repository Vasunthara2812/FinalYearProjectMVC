using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using LearningApp.Models;

namespace LearningApp.Controllers
{
    public class ContentController : Controller
    {
        private const string ApiBaseUrl = "http://localhost:5211/api/GeneratedCourses";
        // 🔥 Change port if needed

        // ===============================
        // 1️⃣ GET - List All Content
        // ===============================
        public async Task<IActionResult> Index()
        {
           ViewBag.Error = "";

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
                return RedirectToAction("Index", "Login");

            // ADD THIS LINE - pass the session user to the view
            ViewBag.UserId = HttpContext.Session.GetString("User");

            using var client = new HttpClient();
            var response = await client.GetAsync(ApiBaseUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var list = JsonSerializer.Deserialize<List<Content>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return View(list ?? new List<Content>());
            }
            else
            {
                ViewBag.Error = "Failed to load content.";
                return View(new List<Content>());
            }
        }

        // ===============================
        // 2️⃣ GET - Show Create Page
        // ===============================
        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        // ===============================
        // 3️⃣ POST - Insert Content
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Create(Content model)
        {
            ViewBag.Error = "";

            using var client = new HttpClient();

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(ApiBaseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ViewBag.Error = "Invalid data. Please check input.";
                return View(model);
            }
            else
            {
                ViewBag.Error = "Internal Server Error. Please try again later.";
                return View(model);
            }
        }
    }
}