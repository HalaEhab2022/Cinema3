using Cinema2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Cinema2.ViewModels;

namespace Cinema2.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ciinema> Ciinemas { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieSubImage> MovieSubImages { get; set; }
        public DbSet<ActorMovie> ActorMovies { get; set; }
        public DbSet<ApplicationUserOTP> applicationUserOTPs { get; set; }
        public DbSet<Cinema2.ViewModels.ValidateOTPVM> ValidateOTPVM { get; set; } = default!;
        public DbSet<Cinema2.ViewModels.NewPasswordVM> NewPasswordVM { get; set; } = default!;

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog= Cinema2;Integrated Security=True;Connect Timeout=30;Encrypt=True;" +
        //        "Trust Server Certificate=True;");
        //}
    }
}
