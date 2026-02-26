using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LearningApp.Models;

namespace LearningApp.Controllers
{
    public class ATSController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "http://localhost:8000";

        public ATSController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: ATS/Check
        public IActionResult Check()
        {
            return View();
        }

        // GET: ATS/Test
        public IActionResult Test()
        {
            return View();
        }

        // POST: ATS/CheckResume
        [HttpPost]
        public async Task<IActionResult> CheckResume(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select a PDF file to upload.");
                return View("Check");
            }

            // Validate file type
            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) 
                && !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Only PDF files are accepted.");
                return View("Check");
            }

            // Validate file size (max 10MB)
            if (file.Length > 10 * 1024 * 1024)
            {
                ModelState.AddModelError("", "File size must be less than 10MB.");
                return View("Check");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromMinutes(3); // Increase timeout for large files
                
                using var content = new MultipartFormDataContent();
                using var memoryStream = new MemoryStream();
                
                // Copy file to memory stream
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                
                var fileContent = new StreamContent(memoryStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                content.Add(fileContent, "file", file.FileName);

                // Make the API call
                var response = await client.PostAsync($"{_apiBaseUrl}/api/ats/score", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    
                    // Log the response for debugging (remove in production)
                    System.Diagnostics.Debug.WriteLine($"API Response: {jsonResponse}");
                    
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };
                    
                    var result = JsonSerializer.Deserialize<ATSResult>(jsonResponse, options);

                    if (result == null)
                    {
                        ModelState.AddModelError("", "Failed to parse the API response.");
                        return View("Check");
                    }

                    ViewBag.FileName = file.FileName;
                    return View("Result", result);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                    ModelState.AddModelError("", $"Error analyzing resume: {response.StatusCode} - {errorContent}");
                    return View("Check");
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Request Exception: {ex.Message}");
                ModelState.AddModelError("", $"Unable to connect to ATS service. Please ensure the API is running on {_apiBaseUrl}. Error: {ex.Message}");
                return View("Check");
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Request Timeout: {ex.Message}");
                ModelState.AddModelError("", "The request timed out. The file might be too large or the server is busy.");
                return View("Check");
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"JSON Parse Exception: {ex.Message}");
                ModelState.AddModelError("", $"Failed to parse API response: {ex.Message}");
                return View("Check");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Exception: {ex.Message}\n{ex.StackTrace}");
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                return View("Check");
            }
        }
    }

    // Models for deserializing the API response
    
}