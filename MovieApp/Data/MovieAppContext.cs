using Microsoft.EntityFrameworkCore;

namespace MovieApp.Data
{
    public class MovieAppContext : DbContext
    {
        public MovieAppContext (DbContextOptions<MovieAppContext> options)
            : base(options)
        {
        }

        public DbSet<MovieApp.Models.Movie> Movie { get; set; } = default!;
    }
}