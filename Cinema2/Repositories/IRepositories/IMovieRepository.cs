using System.Linq.Expressions;

namespace Cinema2.Repositories.IRepositories
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task AddRangeAsync(IEnumerable<Movie> movies, CancellationToken cancellationToken = default);
    }
}
