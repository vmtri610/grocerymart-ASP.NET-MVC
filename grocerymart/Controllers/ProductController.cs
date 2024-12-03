using Microsoft.AspNetCore.Mvc;

namespace grocerymart.Controllers;

public class ProductController : Controller
{
    // GET
    public IActionResult Index(int id)
    {
        return View();
    }
}