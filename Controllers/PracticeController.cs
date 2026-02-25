using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using LearningApp.Models;

namespace LearningApp.Controllers;

public class PracticeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PracticeController> _logger;

    public PracticeController(IHttpClientFactory httpClientFactory, ILogger<PracticeController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
        {
            return RedirectToAction("Index", "Login");
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetUserCourses()
    {
        try
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                return Unauthorized(new { error = "User not logged in" });
            }

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"http://localhost:5211/api/Courses/by-user/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to fetch courses. Status: {response.StatusCode}");
                return StatusCode((int)response.StatusCode, new { error = "Failed to fetch courses" });
            }

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching courses");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetQuiz(int courseId, int numQuestions = 10)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"http://localhost:8000/api/quiz/generate/{courseId}?num_questions={numQuestions}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            else
            {
                _logger.LogError($"API returned error: {response.StatusCode}");
                return StatusCode((int)response.StatusCode, new { error = "Failed to fetch quiz" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching quiz from API");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}