using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;
using Client = Supabase.Client;

namespace grocerymart.Controllers;

public class AuthController : Controller
{
    private readonly Client _supabaseClient;

    public AuthController(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
        Console.WriteLine("AuthController initialized");

        _supabaseClient.Auth.AddStateChangedListener((sender, changed) =>
        {
            Console.WriteLine($"Auth state changed: {changed}");
            Console.WriteLine(sender.CurrentUser?.Id);
            switch (changed)
            {
                case Constants.AuthState.SignedIn:
                    break;
                case Constants.AuthState.SignedOut:
                    break;
                case Constants.AuthState.UserUpdated:
                    break;
                case Constants.AuthState.PasswordRecovery:
                    break;
                case Constants.AuthState.TokenRefreshed:
                    break;
            }
            
            
        });

    }
    
    public IActionResult Index()
    {
        
        return View();
    }
}