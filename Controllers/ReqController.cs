using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FinalYearProjectMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FinalYearProjectMVC.Controllers
{
    [Route("[controller]")]
    public class ReqController : Controller
    {
         private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiBase;

        public ReqController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient    = httpClientFactory.CreateClient();
            _configuration = configuration;
            _apiBase       = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5211";
        }

        // GET: /Requirements
        public async Task<IActionResult> Index()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            System.Console.WriteLine($"Logged in user ID: {userId}");

            var response = await _httpClient.GetAsync(
                $"{_apiBase}/api/Req?userId={userId}");


            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to load requirements from API.";
                return View(new List<RequirementViewModel>());
            }

            var json    = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data    = JsonSerializer.Deserialize<List<RequirementViewModel>>(json, options)
                          ?? new List<RequirementViewModel>();

            return View(data);
        }
    }
}