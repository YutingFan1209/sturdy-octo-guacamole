using Microsoft.AspNetCore.Mvc;
using MovieShopMVC.Models;
using MovieShopMVC.Services;

namespace MovieShopMVC.Controllers;

public class MoviesController(IMovieService tmdb) : Controller
{
    private static readonly List<Purchase> Purchases = [];
    private static readonly Dictionary<int, List<Review>> Reviews = [];
    private static readonly List<Movie> Movies =
    [
        CreateMovie(1, "Avengers: Age of Ultron", "Action", 8.9,
            "https://image.tmdb.org/t/p/w500/4ssDuvEDkSArWEdyBl2X5EHvYKU.jpg",
            "Tony Stark creates a peacekeeping program, but things go wrong when Ultron emerges.",
            new DateTime(2015, 5, 1), 141, 250_000_000M, 1_405_000_000M),
        CreateMovie(2, "Avatar", "Science Fiction", 8.1,
            "https://image.tmdb.org/t/p/w500/kyeqWdyUXW608qlYkRqosgbbJyK.jpg",
            "A former Marine travels to Pandora and becomes involved in the conflict there.",
            new DateTime(2009, 12, 18), 162, 237_000_000M, 2_923_000_000M),
        new Movie
        {
            Id = 3,
            Title = "Titanic",
            PosterUrl = "https://image.tmdb.org/t/p/w500/9xjZS2rlVxm8SFx8kPC3aIGCOYQ.jpg",
            BackdropUrl = "https://image.tmdb.org/t/p/original/9PKZesKMnblFfKxEhQx45YQ2kIe.jpg",
            Overview = "Two passengers from different social classes fall in love aboard the RMS Titanic during its ill-fated maiden voyage.",
            Rating = 8.4,
            Genre = "Drama",
            ReleaseDate = new DateTime(1997, 12, 19),
            Runtime = 194,
            Budget = 200_000_000M,
            Revenue = 2_200_000_000M,
            Price = 9.90M,
            Casts =
            [
                new Cast { Name = "Leonardo DiCaprio", Character = "Jack Dawson" },
                new Cast { Name = "Kate Winslet", Character = "Rose DeWitt Bukater" }
            ],
            Trailers =
            [
                new Trailer { Name = "Official Trailer", Url = "https://www.youtube.com/watch?v=kVrqfYjkTdQ" }
            ]
        },
        CreateMovie(4, "Frozen II", "Animation", 7.2,
            "https://image.tmdb.org/t/p/w500/mINJaa34MtknCYl5AjtNJzWj8cD.jpg",
            "Elsa and her companions travel beyond Arendelle to discover the origin of her powers.",
            new DateTime(2019, 11, 22), 103, 150_000_000M, 1_450_000_000M)
    ];

    public async Task<IActionResult> Index(string? genre, CancellationToken cancellationToken)
    {
        ViewBag.SelectedGenre = genre;
        ViewBag.UsingTmdb = tmdb.IsConfigured;
        IReadOnlyList<Movie> apiMovies = await tmdb.GetPopularMoviesAsync(cancellationToken);
        IEnumerable<Movie> source = apiMovies.Count > 0 ? apiMovies : Movies;
        IEnumerable<Movie> movies = string.IsNullOrWhiteSpace(genre)
            ? source
            : source.Where(m => m.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));

        return View(movies);
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        Movie? movie = await tmdb.GetMovieAsync(id, cancellationToken)
            ?? Movies.FirstOrDefault(m => m.Id == id);
        if (movie is not null && Reviews.TryGetValue(id, out List<Review>? reviews))
        {
            movie.Reviews = reviews;
        }
        return movie is null ? NotFound() : View(movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddReview(Review review)
    {
        if (review.MovieId <= 0)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please enter your name, a rating from 1 to 10, and a short review.";
            return RedirectToAction(nameof(Details), new { id = review.MovieId });
        }

        review.CreatedAt = DateTime.Now;
        if (!Reviews.TryGetValue(review.MovieId, out List<Review>? reviews))
        {
            reviews = [];
            Reviews[review.MovieId] = reviews;
        }
        reviews.Insert(0, review);
        TempData["Success"] = "Thanks! Your review has been posted.";
        return RedirectToAction(nameof(Details), new { id = review.MovieId, anchor = "reviews" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Buy(int movieId, string email, CancellationToken cancellationToken)
    {
        Movie? movie = await tmdb.GetMovieAsync(movieId, cancellationToken)
            ?? Movies.FirstOrDefault(m => m.Id == movieId);
        if (movie is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(email) || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email))
        {
            TempData["Error"] = "Enter a valid email address to complete the purchase.";
            return RedirectToAction(nameof(Details), new { id = movieId });
        }

        var purchase = new Purchase
        {
            MovieId = movie.Id,
            MovieTitle = movie.Title,
            Price = movie.Price,
            Email = email.Trim()
        };
        Purchases.Add(purchase);

        return View("PurchaseComplete", purchase);
    }

    private static Movie CreateMovie(int id, string title, string genre, double rating,
        string posterUrl, string overview, DateTime releaseDate, int runtime,
        decimal budget, decimal revenue)
    {
        return new Movie
        {
            Id = id,
            Title = title,
            Genre = genre,
            Genres = [genre],
            Rating = rating,
            PosterUrl = posterUrl,
            Overview = overview,
            ReleaseDate = releaseDate,
            Runtime = runtime,
            Budget = budget,
            Revenue = revenue,
            Price = 9.90M,
            Trailers = [new Trailer { Name = "Official Trailer", Url = "https://www.youtube.com" }]
        };
    }
}
