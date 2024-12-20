using grocerymart.Models;
using grocerymart.services;
using grocerymart.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Supabase.Postgrest;
using Client = Supabase.Client;

namespace grocerymart.Controllers;

public class ProfileController : Controller
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly Client _supabaseClient;


    public ProfileController(Client supabaseClient, IHubContext<NotificationHub> hubContext)
    {
        _supabaseClient = supabaseClient;
        _hubContext = hubContext;
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

    // update user profile
    public async Task<IActionResult> UpdateProfile(string fullName, string phoneNumber, string username)
    {
        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            await _supabaseClient
                .From<ProfileModel>()
                .Where(x => x.Id == userId)
                .Set(x => x.FullName, fullName)
                .Set(x => x.PhoneNumber, phoneNumber)
                .Set(x => x.Username, username)
                .Update();
            var user = await _supabaseClient.From<ProfileModel>().Select("*")
                .Filter("id", Constants.Operator.Equals, userId).Single();
            var userJson = JsonConvert.SerializeObject(user);
            HttpContext.Session.SetString("User", userJson); // Store serialized user in session

            return RedirectToAction("Index", "Profile");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction("Index", "Profile");
        }
    }

    public async Task<IActionResult> Logout()
    {
        await _supabaseClient.Auth.SignOut();
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> RemoveFromFavourite(string product_id)
    {
        Console.WriteLine("Product Id: " + product_id);
        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Index", "Login");

            await _supabaseClient.Rpc("user_liked_product",
                new Dictionary<string, object> { { "product_id", product_id }, { "user_id", userId } });

            var countLikedProductsResponse = await _supabaseClient.Rpc("count_user_liked_products",
                new Dictionary<string, object> { { "user_id", userId } });

            var countLikedProducts = int.Parse(countLikedProductsResponse.Content);
            HttpContext.Session.SetInt32("LikedProducts", countLikedProducts);

            await _hubContext.Clients.All.SendAsync("ReceiveLikedProducts", countLikedProducts);

            var productsLiked = await _supabaseClient.Rpc<List<ProductResponseModel>>("get_product_in_liked",
                new Dictionary<string, object> { { "p_id", userId } });

            var viewModelLiked = new ProductViewModel
            {
                ProductLiked = productsLiked
            };

            HttpContext.Session.SetString("LikedItems", JsonConvert.SerializeObject(viewModelLiked.ProductLiked));

            await _hubContext.Clients.All.SendAsync("LikedProductsChanged", viewModelLiked.ProductLiked);

            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction("Index", "Home");
        }
    }
}