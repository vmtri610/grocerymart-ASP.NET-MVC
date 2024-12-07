using grocerymart.services;
using Newtonsoft.Json;
using Supabase;
using Supabase.Gotrue;
using Client = Supabase.Client;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllersWithViews();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
builder.Services.AddSignalR();


// Add session services
builder.Services.AddDistributedMemoryCache(); // Required for session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make session cookie HTTP-only
    options.Cookie.IsEssential = true; // Ensure cookie is essential
});

// Add IHttpContextAccessor to access session in middleware
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Retrieve Supabase settings from appsettings.json
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:AnonKey"];

// Create Supabase options (optional settings like AutoConnectRealtime, etc.)
var options = new SupabaseOptions
{
    AutoConnectRealtime = true
};

// Initialize the Supabase client
var supabase = new Client(supabaseUrl, supabaseKey, options);
await supabase.InitializeAsync();


// Register the Supabase client as a singleton so it's available for dependency injection
builder.Services.AddSingleton(supabase);


var app = builder.Build();

// Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Enable session middleware
app.UseAuthorization();
app.MapHub<NotificationHub>("/notificationHub");


app.Services.GetRequiredService<IHttpContextAccessor>(); // Ensures IHttpContextAccessor is available
supabase.Auth.AddStateChangedListener((sender, changed) =>
{
    var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
    var context = httpContextAccessor.HttpContext;

    switch (changed)
    {
        case Constants.AuthState.SignedIn:

            if (sender.CurrentUser != null && context != null)
                context.Session.SetString("UserId", sender.CurrentUser.Id); // Store user ID in session

            break;
        case Constants.AuthState.SignedOut:
            if (context != null)
                // Clear session data on sign out
                context.Session.Clear();
            break;
        case Constants.AuthState.UserUpdated:
            break;
        case Constants.AuthState.PasswordRecovery:
            break;
        case Constants.AuthState.TokenRefreshed:
            break;
    }
});


// Map default controller route
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");


app.Run();