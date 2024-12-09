using grocerymart.Models;
using grocerymart.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Client = Supabase.Client;

namespace grocerymart.Controllers;

public class CheckoutController : Controller
{
    private readonly Client _supabaseClient;

    public CheckoutController(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
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
        Console.WriteLine(productId);
        Console.WriteLine(quantity);
        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Index", "Login");

            await _supabaseClient.Rpc("update_cart_quantity",
                new Dictionary<string, object>
                    { { "p_id", userId }, { "p_prod_id", productId }, { "p_quantity", quantity } });

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return RedirectToAction("Index", "Home");
        }
    }
}