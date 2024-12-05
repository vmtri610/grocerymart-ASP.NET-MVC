using Microsoft.AspNetCore.Mvc;

namespace grocerymart.Controllers;

public class SignUpController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}