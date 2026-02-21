using System;
using System.Collections.Generic;
using System.Net.Http;
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
            // Check session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Login");
            }

            using var client = new HttpClient();

            string endpoint = $"{ApiUrl}/{id}";
            _logger.LogInformation($"Calling API Endpoint: {endpoint}");

            var chapters = new List<Chapters>();

            try
            {
                var response = await client.GetAsync(endpoint);
                var json = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Response Status: {response.StatusCode}");
                _logger.LogInformation($"Response Body: {json}");

                if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(json))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    // Try deserialize as LIST first
                    try
                    {
                        var chapterList = JsonSerializer.Deserialize<List<Chapters>>(json, options);
                        if (chapterList != null && chapterList.Count > 0)
                        {
                            chapters = chapterList;
                        }
                        else
                        {
                            // Try deserialize as SINGLE object
                            var singleChapter = JsonSerializer.Deserialize<Chapters>(json, options);
                            if (singleChapter != null)
                            {
                                chapters.Add(singleChapter);
                            }
                        }
                    }
                    catch (JsonException)
                    {
                        // If list fails, try single object
                        var singleChapter = JsonSerializer.Deserialize<Chapters>(json, options);
                        if (singleChapter != null)
                        {
                            chapters.Add(singleChapter);
                        }
                    }
                }
                else
                {
                    ViewBag.Error = "No chapters found.";
                    _logger.LogWarning("API returned unsuccessful status.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calling API: {ex.Message}");
                ViewBag.Error = "Error loading chapters.";
            }

            return View(chapters);
        }
    }
}
