using Microsoft.AspNetCore.Mvc;

namespace grocerymart.Controllers;

public class LoginController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}