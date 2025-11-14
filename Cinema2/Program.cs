using Cinema2.Configuration;
using Cinema2.Utilities.DBInitializer;
using Stripe;

namespace Cinema2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var ConnectionString =
              builder.Configuration.GetConnectionString("DefaultConnection")
              ?? throw new InvalidOperationException("Connection String"
              + "'DefaultConnection' not found. "
              );

            builder.Services.RegisterConfig(ConnectionString);

            builder.Services.RegisterMapsterConfig();

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

           

            var app = builder.Build();


            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];


            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDBInitializer>();
            service!.Initialize();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
