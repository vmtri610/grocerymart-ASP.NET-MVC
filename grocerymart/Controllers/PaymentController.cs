using Microsoft.AspNetCore.Mvc;

namespace grocerymart.Controllers;

public class PaymentController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}