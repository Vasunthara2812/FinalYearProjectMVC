using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace LearningApp.Controllers
{
    public class LoginController : Controller
    {
        private const string ApiUrl = "http://localhost:5211/api/Login/login";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Error = "";
            using var client = new HttpClient();

            var data = new
            {
                
                Username = username,
                Password = password
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(ApiUrl, content);
            

           

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseJson);

                int userId = result.GetProperty("userId").GetInt32();
                System.Console.WriteLine($"Logged in user ID: {userId}");
                // ✅ Store in session
                HttpContext.Session.SetInt32("UserId", userId);
                HttpContext.Session.SetString("User", username);

                return RedirectToAction("Index", "Home");
            }
            else if(response.StatusCode == System.Net.HttpStatusCode.BadRequest){
                ViewBag.Error = "username or password is reqiured";
                
            return View("Index");
            }
            else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized){
                ViewBag.Error = "Invalid username or password";
            return View("Index");
            }
            else{

            ViewBag.Error = "Internal Server Error. Please try again later.";
            return View("Index");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
