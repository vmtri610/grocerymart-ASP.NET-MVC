using grocerymart.Models;
using grocerymart.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Supabase;

namespace grocerymart.Controllers;

public class HomeController : Controller
{
    private readonly Client _supabaseClient;

    public HomeController(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
        Console.WriteLine();
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (!string.IsNullOrEmpty(userId))
        {
            ViewData["Username"] = userId;
            Console.WriteLine($"User ID: {userId}");
        }
        else
        {
            Console.WriteLine("No user is logged in.");
        }

        try
        {
            var products = await _supabaseClient.From<ProductModel>().Select("*").Get();
            var viewModel = new ProductViewModel { Products = products.Models };
            return View(viewModel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return View();
        }
    }
}