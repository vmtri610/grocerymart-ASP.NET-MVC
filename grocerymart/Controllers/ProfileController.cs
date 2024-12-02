using Microsoft.AspNetCore.Mvc;

namespace grocerymart.Controllers;

public class ProfileController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}