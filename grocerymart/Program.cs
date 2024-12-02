using Supabase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var url = "https://pjukkgikmmeblotolxbf.supabase.co";
var key =
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBqdWtrZ2lrbW1lYmxvdG9seGJmIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzI5NTMwNzksImV4cCI6MjA0ODUyOTA3OX0.qODlWmc9vQCd3G1RrNOy65ACl5a1ACjZlSo43JXFHa4";

var options = new SupabaseOptions
{
    AutoConnectRealtime = true
};

var supabase = new Client(url, key, options);
await supabase.InitializeAsync();

builder.Services.AddSingleton(supabase);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.Run();