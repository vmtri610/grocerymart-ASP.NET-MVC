using grocerymart.Models;
using Microsoft.AspNetCore.Mvc;
using Supabase.Postgrest;
using Client = Supabase.Client;

namespace grocerymart.Controllers;

public class ProductController : Controller
{
    private readonly Client _supabaseClient;

    public ProductController(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
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
}