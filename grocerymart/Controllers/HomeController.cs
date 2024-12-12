using grocerymart.Models;
using grocerymart.services;
using grocerymart.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Supabase.Postgrest;
using Client = Supabase.Client;

namespace grocerymart.Controllers;

public class HomeController : Controller
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<HomeController> _logger;
    private readonly Client _supabaseClient;


    public HomeController(Client supabaseClient, IHubContext<NotificationHub> hubContext,
        ILogger<HomeController> logger)
    {
        _supabaseClient = supabaseClient;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            var user = await _supabaseClient.From<ProfileModel>().Select("*")
                .Filter("id", Constants.Operator.Equals, userId).Single();

            var userJson = JsonConvert.SerializeObject(user);
            HttpContext.Session.SetString("User", userJson); // Store serialized user in session

            var countLikedProductsResponse = await _supabaseClient.Rpc("count_user_liked_products",
                new Dictionary<string, object> { { "user_id", userId } });

            var countLikedProducts = int.Parse(countLikedProductsResponse.Content);
            HttpContext.Session.SetInt32("LikedProducts", countLikedProducts);


            var countCartProductsResponse = await _supabaseClient.Rpc("count_products_in_cart",
                new Dictionary<string, object> { { "p_id", userId } });


            var countCartProducts = int.Parse(countCartProductsResponse.Content);
            HttpContext.Session.SetInt32("TotalCartItems", countCartProducts);


            await _hubContext.Clients.All.SendAsync("ReceiveLikedProducts", countLikedProducts);
            await _hubContext.Clients.All.SendAsync("ReceiveCartProducts", countCartProducts);

            var products = await _supabaseClient.From<ProductModel>().Select("*")
                .Order("created_at", Constants.Ordering.Ascending).Get();
            var viewModel = new ProductViewModel { Products = products.Models };

            var result = await _supabaseClient.Rpc<List<CartItemResponseModel>>("get_products_in_cart",
                new Dictionary<string, object> { { "p_id", userId } });

            var cartViewModel = new CartViewModel
            {
                ProducsInCart = result
            };

            HttpContext.Session.SetString("CartItems", JsonConvert.SerializeObject(cartViewModel.ProducsInCart));

            var productsLiked = await _supabaseClient.Rpc<List<ProductResponseModel>>("get_product_in_liked",
                new Dictionary<string, object> { { "p_id", userId } });

            var viewModelLiked = new ProductViewModel
            {
                ProductLiked = productsLiked
            };

            HttpContext.Session.SetString("LikedItems", JsonConvert.SerializeObject(viewModelLiked.ProductLiked));


            return View(viewModel);
        }
        catch (Exception e)
        {
            return View();
        }
    }

    // liked products

    public async Task<IActionResult> LikedProducts(string product_id)
    {
        try
        {
            var userId = HttpContext.Session.GetString("UserId");

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

            var products = await _supabaseClient.From<ProductModel>().Select("*")
                .Order("created_at", Constants.Ordering.Ascending).Get();
            var viewModel = new ProductViewModel { Products = products.Models };


            return Json(new { success = true, products = viewModel.Products });
        }
        catch (Exception e)
        {
            return Json(new { success = false, error = e.Message });
        }
    }

    // Logout
    public async Task<IActionResult> Logout()
    {
        await _supabaseClient.Auth.SignOut();
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}