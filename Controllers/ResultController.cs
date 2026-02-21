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
    public class ResultController : Controller
    {
        private const string ApiBaseUrl = "http://localhost:5211/api/result";
        private readonly ILogger<ResultController> _logger;

        public ResultController(ILogger<ResultController> logger)
        {
            _logger = logger;
        }

        // id = SubtopicId
        public async Task<IActionResult> Index(int id)
        {
            // Session check
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Index", "Login");
            }

            List<Result> resultList = new List<Result>();

            try
            {
                using (var client = new HttpClient())
                {
                    var apiUrl = $"{ApiBaseUrl}/{id}";

                    _logger.LogInformation("Calling API: {Url}", apiUrl);

                    var response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        _logger.LogInformation("API Response: {Json}", json);

                        // API may return single object
                        if (json.Trim().StartsWith("["))
                        {
                            resultList = JsonSerializer.Deserialize<List<Result>>(json,
                                new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                }) ?? new List<Result>();
                        }
                        else
                        {
                            var single = JsonSerializer.Deserialize<Result>(json,
                                new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });

                            if (single != null)
                                resultList.Add(single);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("API Error: {StatusCode}", response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching result");
            }

            return View(resultList);
        }
    }
}
