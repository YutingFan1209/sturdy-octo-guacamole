using Microsoft.EntityFrameworkCore;
using MovieShop.Api.Data;
using MovieShop.Api.Repositories;
using MovieShop.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<MovieShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieShop")));
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
