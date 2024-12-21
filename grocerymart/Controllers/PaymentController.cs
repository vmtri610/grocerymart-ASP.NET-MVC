using grocerymart.Models;
using grocerymart.services;
using grocerymart.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Supabase.Postgrest;
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

            var addresses = await _supabaseClient.From<AddressModel>().Select("*")
                .Filter("user_id", Constants.Operator.Equals, userId).Get();

            var viewModelAddress = new AddressViewModel
            {
                Addresses = addresses.Models
            };

            HttpContext.Session.SetString("Addresses", JsonConvert.SerializeObject(viewModelAddress.Addresses));

            await _hubContext.Clients.All.SendAsync("AddressAdded", viewModelAddress.Addresses);

            return View(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return RedirectToAction("Index", "Home");
        }
    }

    // payment
    public async Task<IActionResult> Payment()
    {
        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Index", "Login");

            var result = await _supabaseClient.Rpc<List<CartItemResponseModel>>("get_products_in_cart",
                new Dictionary<string, object> { { "p_id", userId } });

            // insert to order history
            var orderHistory = new OrderHistoryModel
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = result.Sum(x => x.Price * x.Quantity)
            };

            var insertedOrderHistory = await _supabaseClient.From<OrderHistoryModel>().Insert(orderHistory);

            Console.WriteLine("Inserted Order History: " + insertedOrderHistory.Models[0].Id);

            // insert to order items
            foreach (var item in result)
            {
                var orderItem = new OrderItemsModel
                {
                    OrderId = insertedOrderHistory.Models[0].Id,
                    ProductId = item.ProdId,
                    Quantity = item.Quantity
                };

                await _supabaseClient.From<OrderItemsModel>().Insert(orderItem);
            }

            // delete from cart where user_id = userId
            await _supabaseClient.From<CartModel>().Where(x => x.UserId == userId).Delete();
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return RedirectToAction("Index", "Home");
        }
    }
}