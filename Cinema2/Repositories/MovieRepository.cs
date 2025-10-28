using Cinema2.Repositories.IRepositories;

namespace Cinema2.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private ApplicationDbContext _context;//= new();

        public MovieRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<Movie> movies, CancellationToken cancellationToken = default)
        {
            await _context.AddRangeAsync(movies, cancellationToken);
        }
    }
}
