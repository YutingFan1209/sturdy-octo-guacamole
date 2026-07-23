using Microsoft.EntityFrameworkCore;
using MovieShop.Api.Data;
using MovieShop.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<MovieShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieShop")));
builder.Services.AddScoped<IMovieRepository, MovieRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
