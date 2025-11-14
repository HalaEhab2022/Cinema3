












using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema2.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {

        public HomeController(ApplicationDbContext context, IMovieRepository movieRepository, IRepository<Category> categoryRepository, 
            IRepository<Ciinema> ciinemaRepository, IRepository<Actor> actorRepository, IRepository<ActorMovie> actorMovieRepository, 
            IRepository<MovieSubImage> movieSubImgRepository)
        {
            _context = context;
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _ciinemaRepository = ciinemaRepository;
            _actorRepository = actorRepository;
            _actorMovieRepository = actorMovieRepository;
            _movieSubImgRepository = movieSubImgRepository;
        }

        private readonly ApplicationDbContext _context;
        private readonly IMovieRepository _movieRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Ciinema> _ciinemaRepository;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<ActorMovie> _actorMovieRepository;
        private readonly IRepository<MovieSubImage> _movieSubImgRepository;
        public async Task<IActionResult> Index(FilterMovieVM filterMovieVM , CancellationToken cancellationToken, int page = 1)
        {
            var movies =await _movieRepository.GetAsync(includes: [e=>e.ciinema, e=>e.category], tracked: false, cancellationToken: cancellationToken);

            if (filterMovieVM.name is not null)
            {
                movies = movies.Where(e => e.Name.Contains(filterMovieVM.name.Trim()));
                ViewBag.name = filterMovieVM.name;
            }
            if (filterMovieVM.categoryId is not null)
            {
                movies = movies.Where(e => e.CategoryId == filterMovieVM.categoryId);
                ViewBag.categoryId = filterMovieVM.categoryId;
            }
            if (filterMovieVM.ciinemaId is not null)
            {
                movies = movies.Where(e => e.CiinemaId == filterMovieVM.ciinemaId);
                ViewBag.ciinemaId = filterMovieVM.ciinemaId;
            }

            var categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.categories = categories.AsEnumerable();

            var ciinemas = await _ciinemaRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.ciinemas = ciinemas.AsEnumerable();

            ViewBag.TotalPages = Math.Ceiling(movies.Count() / 6.0);
            ViewBag.currentPage = page;
            movies = movies.Skip((page - 1) * 6).Take(6);

            return View(movies.AsEnumerable());
        }

        public async Task<IActionResult> Details(FilterMovieVM filterMovieVM, int id , CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id, includes: [e => e.category, e => e.ciinema , e=>e.subImages],tracked:false, cancellationToken: cancellationToken);

            if (movie is null)
            {
                return NotFound();
            }
            movie.Traffic += 1;
            _context.SaveChanges();

            var relatedMovies = await _context.Movies.Where(e => e.Name.Contains(movie.Name) && e.Id != movie.Id).OrderBy(e => e.Traffic).Skip(0).Take(4).ToListAsync();

            return View(new MovieWithRelatedVM
            {
                Movie = movie,
                RelatedMovies = relatedMovies
            });

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
