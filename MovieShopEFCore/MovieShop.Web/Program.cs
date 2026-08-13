var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<MovieShopMVC.Services.ApiBearerTokenHandler>();
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "MovieShopAuthCookie";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });
builder.Services.AddHttpClient<MovieShopMVC.Services.IMovieService, MovieShopMVC.Services.MovieShopApiService>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["MovieShopApi:BaseUrl"]
        ?? "http://127.0.0.1:5080/");
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
}).AddHttpMessageHandler<MovieShopMVC.Services.ApiBearerTokenHandler>();
builder.Services.AddScoped<MovieShopMVC.Services.IMovieRankingService, MovieShopMVC.Services.MovieRankingService>();
builder.Services.AddHttpClient<MovieShopMVC.Services.IAccountService, MovieShopMVC.Services.AccountApiService>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["MovieShopApi:BaseUrl"]
        ?? "http://127.0.0.1:5080/");
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
}).AddHttpMessageHandler<MovieShopMVC.Services.ApiBearerTokenHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
