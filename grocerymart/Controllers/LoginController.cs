using Microsoft.AspNetCore.Mvc;
using Supabase;

namespace GroceryMart.Controllers;

public class LoginController : Controller
{
    private readonly Client _supabaseClient;

    public LoginController(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Login(string email, string password)
    {
        try
        {
            // Attempt login with Supabase
            var session = await _supabaseClient.Auth.SignIn(email, password);

            if (session?.User == null)
            {
                // Login failed, user not found or invalid credentials
                TempData["LoginError"] = "Invalid email or password. Please try again.";
                return RedirectToAction("Index");
            }

            // Redirect to Home page if login is successful
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., network issues, service unavailability, etc.)
            TempData["LoginError"] = "An error occurred while logging in. Please try again later.";
            return RedirectToAction("Index");
        }
    }
}