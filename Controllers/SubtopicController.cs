using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LearningApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LearningApp.Controllers
{
    public class SubtopicController : Controller
    {
        private const string BaseApiUrl = "http://localhost:5211/api/subtopic";
        private readonly ILogger<SubtopicController> _logger;

        public SubtopicController(ILogger<SubtopicController> logger)
        {
            _logger = logger;
        }

        // id = chapterId
        public async Task<IActionResult> Index(int id)
        {
            // ✅ Session check
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Login");
            }

            if (id <= 0)
            {
                _logger.LogWarning("Invalid chapterId received: {ChapterId}", id);
                return View(new List<Subtopics>());
            }

            var subtopics = new List<Subtopics>();

            try
            {
                using var client = new HttpClient();

                // ✅ Correct API route
                var apiUrl = $"{BaseApiUrl}/chapter/{id}";

                _logger.LogInformation("Calling API: {ApiUrl}", apiUrl);

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    _logger.LogInformation("API Response: {Json}", json);

                    subtopics = JsonSerializer.Deserialize<List<Subtopics>>(json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }) ?? new List<Subtopics>();

                    _logger.LogInformation("Subtopics Count: {Count}", subtopics.Count);
                }
                else
                {
                    _logger.LogError("API call failed. Status Code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while calling Subtopic API");
            }

            return View(subtopics);
        }
    }
}
