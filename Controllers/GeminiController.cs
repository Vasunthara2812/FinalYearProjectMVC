using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinalYearProjectMVC.Controllers
{
    public class GeminiController : Controller
    {
        private readonly IConfiguration _configuration;

        public GeminiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

         [HttpPost]
        public async Task<IActionResult> AskGemini(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                ViewBag.Response = "Please enter a prompt.";
                return View("Index");
            }

            var apiKey = _configuration["Gemini:ApiKey"];
            Console.WriteLine($"Using API Key: {apiKey}");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro:generateContent?key={apiKey}";
            
            using var client = new HttpClient();

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Gemini API Response:");
            Console.WriteLine(responseString);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Response = $"HTTP Error: {response.StatusCode}\n{responseString}";
                return View("Index");
            }

            string output = "No response received.";

            using (JsonDocument doc = JsonDocument.Parse(responseString))
            {
                var root = doc.RootElement;

                if (root.TryGetProperty("error", out JsonElement error))
                {
                    output = "API Error: " + error.GetProperty("message").GetString();
                }
                else if (root.TryGetProperty("candidates", out JsonElement candidates))
                {
                    output = candidates[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();
                }
            }

            ViewBag.Response = output;
            return View("Index");
        }
    
    }
}