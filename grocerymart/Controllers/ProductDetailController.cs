using Microsoft.AspNetCore.Mvc;

namespace grocerymart.Controllers;

public class ProductDetailController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}