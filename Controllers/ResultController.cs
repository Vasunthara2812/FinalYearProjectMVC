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
    public class ResultController : Controller
    {
        private const string ApiUrl = "http://localhost:5211/api/result";
        private readonly ILogger<ResultController> _logger;

        public ResultController(ILogger<ResultController> logger)
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

            if(id > 0)
                queryParams.Add($"courseId={id}");



            if (queryParams.Any())
                apiUrl += "?" + string.Join("&", queryParams);
            Console.WriteLine($"API URL: {apiUrl}");
            Console.WriteLine($"CourseId: {id}");
            var response = await client.GetAsync(apiUrl);

            var list = new List<Result>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response JSON: {json}");

                list = JsonSerializer.Deserialize<List<Result>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<Result>();
                    Console.WriteLine($"Deserialized List Count: {list.Count}");

                
            }

           

            return View(list);
        }


    }
}