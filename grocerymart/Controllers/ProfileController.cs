using grocerymart.Models;
using grocerymart.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Supabase.Postgrest;
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
}