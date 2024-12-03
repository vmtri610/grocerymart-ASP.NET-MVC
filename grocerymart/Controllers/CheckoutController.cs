using Microsoft.AspNetCore.Mvc;

namespace grocerymart.Controllers;

public class CheckoutController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}