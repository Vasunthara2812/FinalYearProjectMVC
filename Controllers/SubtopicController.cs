using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using LearningApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LearningApp.Controllers
{
    public class SubtopicController : Controller
    {
        private const string ApiUrl = "http://localhost:5211/api/subtopic";
        private readonly ILogger<SubtopicController> _logger;

        public SubtopicController(ILogger<SubtopicController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(int id)
        {
            // Session check
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Login");
            }

            using var client = new HttpClient();

            // ✅ Correct API URL formation (Route Parameter)
            var apiUrl = ApiUrl;

            if (id > 0)
            {
                apiUrl = $"{ApiUrl}/{id}";
            }

            Console.WriteLine($"API URL: {apiUrl}");

            var response = await client.GetAsync(apiUrl);

            Console.WriteLine($"API Response Status: {response.StatusCode}");

            var list = new List<Subtopics>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response JSON: {json}");

                list = JsonSerializer.Deserialize<List<Subtopics>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Subtopics>();

                Console.WriteLine($"Deserialized List Count: {list.Count}");
            }
            else
            {
                Console.WriteLine("API call failed.");
            }

            return View(list);
        }
    }
}
