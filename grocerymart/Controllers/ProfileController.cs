using grocerymart.Models;
using grocerymart.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Client = Supabase.Client;

namespace grocerymart.Controllers;

public class ProfileController : Controller
{
    private readonly Client _supabaseClient;

    public ProfileController(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var productsLiked = await _supabaseClient.Rpc<List<ProductResponseModel>>("get_product_in_liked",
            new Dictionary<string, object> { { "p_id", userId } });

        var viewModelLiked = new ProductViewModel
        {
            ProductLiked = productsLiked
        };

        HttpContext.Session.SetString("LikedItems", JsonConvert.SerializeObject(viewModelLiked.ProductLiked));
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await _supabaseClient.Auth.SignOut();
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}