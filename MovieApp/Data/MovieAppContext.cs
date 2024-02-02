using Microsoft.EntityFrameworkCore;

namespace MovieApp.Data;

public class MovieAppContext : DbContext
{
    public MovieAppContext(DbContextOptions<MovieAppContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    public DbSet<Models.Movie> Movie { get; set; } = default!;
}