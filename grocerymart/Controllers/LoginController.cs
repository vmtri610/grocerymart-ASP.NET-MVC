using Microsoft.AspNetCore.Mvc;
using Client = Supabase.Client;

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
        var session = await _supabaseClient.Auth.SignIn(email, password);

        Console.WriteLine(session?.User?.Email);

        return RedirectToAction("Index", "Home");
    }
}