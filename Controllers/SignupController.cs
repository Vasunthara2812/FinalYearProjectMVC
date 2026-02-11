using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace LearningApp.Controllers
{
    public class SignupController : Controller
    {
        private const string ApiUrl = "http://localhost:5211/api/Login/signup";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(string username, string password)
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
                // Store username in session
                HttpContext.Session.SetString("User", username);
                return RedirectToAction("Index", "Login");
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

        
    }
}
