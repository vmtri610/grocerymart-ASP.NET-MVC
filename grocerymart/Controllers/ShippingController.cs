using Microsoft.AspNetCore.Mvc;

namespace grocerymart.Controllers;

public class ShippingController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}