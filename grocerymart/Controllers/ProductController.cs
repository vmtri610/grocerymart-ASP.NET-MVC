using grocerymart.Models;
using grocerymart.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Supabase.Postgrest;
using Client = Supabase.Client;

namespace grocerymart.Controllers;

public class ProductController : Controller
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<HomeController> _logger;
    private readonly Client _supabaseClient;


    public ProductController(Client supabaseClient, IHubContext<NotificationHub> hubContext,  ILogger<HomeController> logger)
    {
        _supabaseClient = supabaseClient;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string id)
    {
        try
        {
            var product = await _supabaseClient
                .From<ProductModel>()
                .Select("*")
                .Filter("id", Constants.Operator.Equals, id) // Use Filter instead of Eq
                .Single();

            Console.WriteLine(product);
            // Đảm bảo trả về đối tượng với kiểu rõ ràng cho View
            return View(product); // Ép kiểu rõ ràng nếu cần
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> Like(string id)
    {
        Console.WriteLine("Like product with id: " + id);
        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            await _supabaseClient.Rpc("user_liked_product",
                new Dictionary<string, object> { { "product_id", id }, { "user_id", userId } });

            var countLikedProductsResponse = await _supabaseClient.Rpc("count_user_liked_products",
                new Dictionary<string, object> { { "user_id", userId } });

            var countLikedProducts = int.Parse(countLikedProductsResponse.Content);
            HttpContext.Session.SetInt32("LikedProducts", countLikedProducts);

            
            _logger.LogInformation("Sending SignalR event: ReceiveLikedProducts with count = {countLikedProducts}", countLikedProducts);
            await _hubContext.Clients.All.SendAsync("ReceiveLikedProducts", countLikedProducts);
            _logger.LogInformation("SignalR event sent successfully.");

            return Json(new { success = true });
        }
        catch (Exception e)
        {
            return Json(new { success = false, error = e.Message });
        }
    }
}