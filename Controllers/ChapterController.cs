using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LearningApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LearningApp.Controllers
{
    public class ChapterController : Controller
    {
        private const string ApiUrl = "http://localhost:5211/api/chapter";
        private readonly ILogger<ChapterController> _logger;

        public ChapterController(ILogger<ChapterController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(int id)
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
            if(id > 0)
                queryParams.Add($"courseId={id}");


            if (queryParams.Any())
                apiUrl += "?" + string.Join("&", queryParams);
            Console.WriteLine($"API URL: {apiUrl}");
            Console.WriteLine($"CourseId: {id}");
            var response = await client.GetAsync(apiUrl);

            var list = new List<Chapters>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                list = JsonSerializer.Deserialize<List<Chapters>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<Chapters>();

                
            }

           

            return View(list);
        }


    }
}