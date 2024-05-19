using Microsoft.EntityFrameworkCore;

namespace MystMovie.DB
{
    public class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
    }
}
