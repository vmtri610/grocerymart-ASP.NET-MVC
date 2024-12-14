using grocerymart.Models;
using grocerymart.services;
using grocerymart.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Client = Supabase.Client;

namespace grocerymart.Controllers;

public class PaymentController : Controller
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly Client _supabaseClient;

    public PaymentController(Client supabaseClient, IHubContext<NotificationHub> hubContext)
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
}