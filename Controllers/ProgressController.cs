using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LearningApp.Models;

namespace LearningApp.Controllers;

public class ProgressController : Controller
{

    public IActionResult Index()
    {
         if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
        {
            return RedirectToAction("Index", "Login");
        }
        return View();
    }
}