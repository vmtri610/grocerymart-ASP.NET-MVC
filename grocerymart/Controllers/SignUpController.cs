using grocerymart.Models;
using Microsoft.AspNetCore.Mvc;
using Client = Supabase.Client;

namespace grocerymart.Controllers;

public class SignUpController : Controller
{
    private readonly Client _supabaseClient;

    public SignUpController(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> SignUp(string email, string password, string fullName, string phoneNumber,
        string username)
    {
        var session = await _supabaseClient.Auth.SignUp(email, password);

        // Check if the user was successfully created, update the user's full name
        if (session?.User != null)
        {
            var user = await _supabaseClient
                .From<ProfileModel>()
                .Where(x => x.Id == session.User.Id)
                .Set(x => x.FullName, fullName)
                .Set(x => x.PhoneNumber, phoneNumber)
                .Set(x => x.Username, username)
                .Update();
        }


        return RedirectToAction("Index", "Login");
    }
}