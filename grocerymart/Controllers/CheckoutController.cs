using grocerymart.Models;
using grocerymart.services;
using grocerymart.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Client = Supabase.Client;

namespace grocerymart.Controllers;

public class CheckoutController : Controller
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly Client _supabaseClient;


    public CheckoutController(Client supabaseClient, IHubContext<NotificationHub> hubContext)
    {
        _supabaseClient = supabaseClient;
        _hubContext = hubContext;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Index", "Login");

            var result = await _supabaseClient.Rpc<List<CartItemResponseModel>>("get_products_in_cart",
                new Dictionary<string, object> { { "p_id", userId } });


            var viewModel = new CartViewModel
            {
                ProducsInCart = result
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return RedirectToAction("Index", "Home");
        }
    }

    // increment and decrement quantity of product in cart
    public async Task<IActionResult> UpdateQuantity(string productId, int quantity)
    {
        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Index", "Login");

            await _supabaseClient.Rpc("update_cart_quantity",
                new Dictionary<string, object>
                    { { "p_id", userId }, { "p_prod_id", productId }, { "p_quantity", quantity } });

            var countCartProductsResponse = await _supabaseClient.Rpc("count_products_in_cart",
                new Dictionary<string, object> { { "p_id", userId } });


            var countCartProducts = int.Parse(countCartProductsResponse.Content);
            HttpContext.Session.SetInt32("TotalCartItems", countCartProducts);
            await _hubContext.Clients.All.SendAsync("ReceiveCartProducts", countCartProducts);

            var result = await _supabaseClient.Rpc<List<CartItemResponseModel>>("get_products_in_cart",
                new Dictionary<string, object> { { "p_id", userId } });

            var cartViewModel = new CartViewModel
            {
                ProducsInCart = result
            };

            HttpContext.Session.SetString("CartItems", JsonConvert.SerializeObject(cartViewModel.ProducsInCart));

            await _hubContext.Clients.All.SendAsync("CartProductsChanged", cartViewModel.ProducsInCart);

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return RedirectToAction("Index", "Home");
        }
    }
}