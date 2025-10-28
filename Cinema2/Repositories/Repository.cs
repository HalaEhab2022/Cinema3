using System.Linq.Expressions;
using Cinema2.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Cinema2.Repositories
{
    public class Repository<T> : IRepository<T> where T: class                 
    {
        private ApplicationDbContext _context;// = new();
        private DbSet<T> _db;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }


        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _db.AddAsync(entity, cancellationToken);
            return entity;
        }
        public void Update(T entity)
        {
            _db.Update(entity);
        }
        public void Delete(T entity)
        {
            _db.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default)
        {
            var entities = _db.AsQueryable();

            if (expression is not null)
                entities = entities.Where(expression);

            if (includes is not null)
            {
                foreach (var item in includes)
                {
                    entities = entities.Include(item);
                }
            }

            if (!tracked)
            {
                entities = entities.AsNoTracking();
            }

            return await entities.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default)
        {
            return (await GetAsync(expression, includes, tracked, cancellationToken)).FirstOrDefault();
        }


        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error :{ex.Message}");
            }
        }
    }
}
