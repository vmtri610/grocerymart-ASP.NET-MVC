using Microsoft.AspNetCore.Mvc;

namespace grocerymart.Controllers;

public class FavouriteLIstController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}