using Microsoft.EntityFrameworkCore;
using MovieShop.Api.Models;

namespace MovieShop.Api.Data;

public class MovieShopDbContext(DbContextOptions<MovieShopDbContext> options)
    : DbContext(options)
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<MovieGenre> MovieGenres => Set<MovieGenre>();
    public DbSet<CastMember> CastMembers => Set<CastMember>();
    public DbSet<MovieCast> MovieCasts => Set<MovieCast>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<Trailer> Trailers => Set<Trailer>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<CrewMember> CrewMembers => Set<CrewMember>();
    public DbSet<MovieCrew> MovieCrews => Set<MovieCrew>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.ToTable("Movie");
            entity.Property(movie => movie.Title).HasMaxLength(256).IsRequired();
            entity.Property(movie => movie.Overview).HasColumnName("OverView");
            entity.Property(movie => movie.Tagline).HasMaxLength(512);
            entity.Property(movie => movie.Budget).HasPrecision(18, 4);
            entity.Property(movie => movie.Revenue).HasPrecision(18, 4);
            entity.Property(movie => movie.Price).HasPrecision(5, 2);
            entity.Property(movie => movie.OriginalLanguage).HasMaxLength(64);
            entity.Property(movie => movie.UpdatedBy).HasMaxLength(128);
            entity.Property(movie => movie.CreatedBy).HasMaxLength(128);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.ToTable("Genre");
            entity.Property(genre => genre.Name).HasMaxLength(64).IsRequired();
        });

        modelBuilder.Entity<MovieGenre>(entity =>
        {
            entity.ToTable("MovieGenre");
            entity.HasKey(movieGenre => new { movieGenre.MovieId, movieGenre.GenreId });

            entity.HasOne(movieGenre => movieGenre.Movie)
                .WithMany(movie => movie.MovieGenres)
                .HasForeignKey(movieGenre => movieGenre.MovieId);

            entity.HasOne(movieGenre => movieGenre.Genre)
                .WithMany(genre => genre.MovieGenres)
                .HasForeignKey(movieGenre => movieGenre.GenreId);
        });

        modelBuilder.Entity<CastMember>(entity =>
        {
            entity.ToTable("Cast");
            entity.Property(castMember => castMember.Name).HasMaxLength(128);
            entity.Property(castMember => castMember.ProfilePath).HasMaxLength(2084);
        });

        modelBuilder.Entity<MovieCast>(entity =>
        {
            entity.ToTable("MovieCast");
            entity.HasKey(movieCast => new
            {
                movieCast.MovieId,
                movieCast.CastId,
                movieCast.Character
            });
            entity.Property(movieCast => movieCast.Character)
                .HasMaxLength(450)
                .IsRequired();

            entity.HasOne(movieCast => movieCast.Movie)
                .WithMany(movie => movie.MovieCasts)
                .HasForeignKey(movieCast => movieCast.MovieId);

            entity.HasOne(movieCast => movieCast.CastMember)
                .WithMany(castMember => castMember.MovieCasts)
                .HasForeignKey(movieCast => movieCast.CastId);
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("User");
            entity.Property(user => user.FirstName).HasMaxLength(128);
            entity.Property(user => user.LastName).HasMaxLength(128);
            entity.Property(user => user.DateOfBirth).HasColumnType("date");
            entity.Property(user => user.Email).HasMaxLength(256);
            entity.Property(user => user.HashedPassword).HasMaxLength(1024);
            entity.Property(user => user.Salt).HasMaxLength(1024);
            entity.Property(user => user.PhoneNumber).HasMaxLength(16);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Review");
            entity.HasKey(review => new { review.MovieId, review.UserId });
            entity.Property(review => review.Rating).HasPrecision(3, 2);

            entity.HasOne(review => review.Movie)
                .WithMany(movie => movie.Reviews)
                .HasForeignKey(review => review.MovieId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(review => review.User)
                .WithMany(user => user.Reviews)
                .HasForeignKey(review => review.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.ToTable("Purchase");
            entity.Property(purchase => purchase.PurchaseNumber)
                .HasMaxLength(450)
                .IsRequired();
            entity.Property(purchase => purchase.TotalPrice).HasPrecision(18, 2);

            entity.HasOne(purchase => purchase.Movie)
                .WithMany(movie => movie.Purchases)
                .HasForeignKey(purchase => purchase.MovieId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(purchase => purchase.User)
                .WithMany(user => user.Purchases)
                .HasForeignKey(purchase => purchase.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Trailer>(entity =>
        {
            entity.ToTable("Trailer");
            entity.Property(trailer => trailer.Name)
                .HasMaxLength(2084)
                .IsRequired();
            entity.Property(trailer => trailer.TrailerUrl)
                .HasMaxLength(2084)
                .IsRequired();

            entity.HasOne(trailer => trailer.Movie)
                .WithMany(movie => movie.Trailers)
                .HasForeignKey(trailer => trailer.MovieId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.ToTable("Favorites");

            entity.HasOne(favorite => favorite.Movie)
                .WithMany(movie => movie.Favorites)
                .HasForeignKey(favorite => favorite.MovieId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(favorite => favorite.User)
                .WithMany(user => user.Favorites)
                .HasForeignKey(favorite => favorite.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<CrewMember>(entity =>
        {
            entity.ToTable("Crew");
            entity.Property(crewMember => crewMember.Name).HasMaxLength(128);
            entity.Property(crewMember => crewMember.ProfilePath).HasMaxLength(2084);
        });

        modelBuilder.Entity<MovieCrew>(entity =>
        {
            entity.ToTable("MovieCrew");
            entity.HasKey(movieCrew => new
            {
                movieCrew.MovieId,
                movieCrew.CrewId,
                movieCrew.Department,
                movieCrew.Job
            });
            entity.Property(movieCrew => movieCrew.Department)
                .HasMaxLength(128)
                .IsRequired();
            entity.Property(movieCrew => movieCrew.Job)
                .HasMaxLength(128)
                .IsRequired();

            entity.HasOne(movieCrew => movieCrew.Movie)
                .WithMany(movie => movie.MovieCrews)
                .HasForeignKey(movieCrew => movieCrew.MovieId);

            entity.HasOne(movieCrew => movieCrew.CrewMember)
                .WithMany(crewMember => crewMember.MovieCrews)
                .HasForeignKey(movieCrew => movieCrew.CrewId);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");
            entity.Property(role => role.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRole");
            entity.HasKey(userRole => new { userRole.UserId, userRole.RoleId });

            entity.HasOne(userRole => userRole.User)
                .WithMany(user => user.UserRoles)
                .HasForeignKey(userRole => userRole.UserId);

            entity.HasOne(userRole => userRole.Role)
                .WithMany(role => role.UserRoles)
                .HasForeignKey(userRole => userRole.RoleId);
        });
    }
}
