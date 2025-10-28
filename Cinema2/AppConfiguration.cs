using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cinema2
{
    public static class AppConfiguration
    {
        public static void RegisterConfig(this IServiceCollection services, string connection)
        {
           
            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Ciinema>, Repository<Ciinema>>();
            services.AddScoped<IRepository<Movie>, Repository<Movie>>();
            services.AddScoped<IRepository<Actor>, Repository<Actor>>();
            services.AddScoped<IRepository<ActorMovie>, Repository<ActorMovie>>();
            services.AddScoped<IRepository<MovieSubImage>, Repository<MovieSubImage>>();
            services.AddScoped<IMovieRepository, MovieRepository>();
           

            services.AddDbContext<ApplicationDbContext>(option =>
            {
                //option.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]);
                //option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
                option.UseSqlServer(connection);
            });
        }
    }
}
