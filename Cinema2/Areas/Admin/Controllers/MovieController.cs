using System.Threading;
using System.Threading.Tasks;
using Cinema2.Models;
using Cinema2.Repositories.IRepositories;
using Cinema2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Cinema2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMovieRepository _movieRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Ciinema> _ciinemaRepository;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<ActorMovie> _actorMovieRepository;
        private readonly IRepository<MovieSubImage> _movieSubImgRepository;

        public MovieController(ApplicationDbContext context, IMovieRepository movieRepository, IRepository<Category> categoryRepository,
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

        public async Task<IActionResult> Index(FilterMovieVM filterMovieVM,CancellationToken cancellationToken ,int page = 1)
        {
           
            var movies =await _movieRepository.GetAsync(includes: [e=>e.ciinema, e=>e.category], tracked: false, cancellationToken: cancellationToken);

            if(filterMovieVM.name is not null)
            {
                movies = movies.Where(e => e.Name.Contains(filterMovieVM.name.Trim()));
                ViewBag.name = filterMovieVM.name;
            }
            if(filterMovieVM.minPrice is not null)
            {
                movies = movies.Where(e => e.TicketPrice >= filterMovieVM.minPrice);
                ViewBag.minPrice = filterMovieVM.minPrice;
            }
            if (filterMovieVM.maxPrice is not null)
            {
                movies = movies.Where(e => e.TicketPrice <= filterMovieVM.maxPrice);
                ViewBag.maxPrice = filterMovieVM.maxPrice;
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

            var categories =await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.categories = categories.AsEnumerable();

            var ciinemas =await _ciinemaRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.ciinemas = ciinemas.AsEnumerable();

            #region pagination
            ViewBag.TotalPages = Math.Ceiling(movies.Count() / 5.0);
            ViewBag.currentPage = page;
            movies = movies.Skip((page - 1) * 5).Take(5);
            #endregion

            return View(movies.AsEnumerable());
        }


        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var categories =await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
            var ciinemas =await _ciinemaRepository.GetAsync(cancellationToken: cancellationToken);
            var actors =await _actorRepository.GetAsync(cancellationToken: cancellationToken);
            return View(new CategoriesWithCiinemasVM
            {
                Categories = categories.AsEnumerable(),
                Ciinemas = ciinemas.AsEnumerable(),
                Actors= actors.AsEnumerable()
            });
        }


        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile img, List<IFormFile>? subImgs, List<int> SelectedActorIds, CancellationToken cancellationToken)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                if (img is not null && img.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        img.CopyTo(stream);
                    }

                    movie.MainImg = fileName;
                }

                var movieCreated =await _movieRepository.AddAsync(movie, cancellationToken);
                await _movieRepository.CommitAsync(cancellationToken);

                if (subImgs is not null && subImgs.Count > 0)
                {
                    foreach (var item in subImgs)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie-images", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            item.CopyTo(stream);
                        }

                        await _movieSubImgRepository.AddAsync(new()
                        {
                            Img = fileName,
                            movieId = movieCreated.Id
                        }, cancellationToken: cancellationToken);
                    }

                    await _movieSubImgRepository.CommitAsync(cancellationToken);
                }

                if (SelectedActorIds != null && SelectedActorIds.Any())
                {
                    foreach (var actorId in SelectedActorIds)
                    {
                        await _actorMovieRepository.AddAsync(new ActorMovie
                        {
                            ActorId = actorId,
                            MovieId = movieCreated.Id
                        },cancellationToken: cancellationToken);
                    }
                     await _actorMovieRepository.CommitAsync(cancellationToken);
                }

                TempData["success-notification"] = "Add Movie Successfully";
                transaction.Commit();
            }
            catch (Exception ex)
            {
                TempData["error-notification"] = "Error While Saving Movie";
                transaction.Rollback();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var movie =await _movieRepository.GetOneAsync(e => e.Id == id, includes: [e=>e.ActorMovies, e=>e.subImages], cancellationToken:cancellationToken);
            if (movie is null)
                return RedirectToAction("NotFoundPage", "Home");

           
            var actorMovies = await _actorMovieRepository.GetAsync(cancellationToken: cancellationToken);
            var categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
            var ciinemas = await _ciinemaRepository.GetAsync(cancellationToken: cancellationToken);
            var actors = await _actorRepository.GetAsync(cancellationToken: cancellationToken);

            return View(new CategoriesWithCiinemasVM
            {
                Categories = categories.AsEnumerable(),
                Ciinemas = ciinemas.AsEnumerable(),
                ActorMovies= actorMovies.AsEnumerable(),
                Actors= actors.AsEnumerable(),
                Movie = movie
            });

        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(Movie movie, IFormFile? img, List<IFormFile>? subImgs, CancellationToken cancellationToken)
        {
            var movieInDb =await _movieRepository.GetOneAsync(e => e.Id == movie.Id, includes: [e => e.ActorMovies], tracked: false, cancellationToken: cancellationToken);

            if (movieInDb is null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            if (img is not null)
            {
                if (img.Length > 0)
                {
                    //save image in wwwroot
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        img.CopyTo(stream);
                    }

                    //remove old image from wwwroot
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", movieInDb.MainImg);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                    //save image in db
                    movie.MainImg = fileName;
                }
            }
            else
            {
                movie.MainImg = movieInDb.MainImg;
            }

            _movieRepository.Update(movie);
            await _movieRepository.CommitAsync(cancellationToken);

            if (subImgs is not null && subImgs.Count > 0)
            {
                movie.subImages = new List<MovieSubImage>();
                foreach (var item in subImgs)
                {
                    //save image in wwwroot
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie-images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }

                    await _movieSubImgRepository.AddAsync(new()
                    {
                        Img = fileName,
                        movieId = movie.Id
                    }, cancellationToken: cancellationToken);
                }
                await _movieSubImgRepository.CommitAsync(cancellationToken);
            }

            TempData["success-notification"] = "Edit Movie Successfully";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var movie =await _movieRepository.GetOneAsync(e => e.Id == id, includes: [e=>e.ActorMovies, e=>e.subImages], cancellationToken:cancellationToken);

            if (movie is null)
                return RedirectToAction("NotFoundPage", "Home");

            //remove old image in wwwroot
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", movie.MainImg);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }

            foreach (var item in movie.subImages)
            {
                //remove old image in wwwroot
                var subImgOldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie-images", item.Img);
                if (System.IO.File.Exists(subImgOldPath))
                {
                    System.IO.File.Delete(subImgOldPath);
                }
            }

            _movieRepository.Delete(movie);
            await _movieRepository.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Delete Movie Successfully";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> DeleteSubImg(int movieId, string Img, CancellationToken cancellationToken)
        {
            var MovieSubImgInDb = await _movieSubImgRepository.GetOneAsync(e=>e.movieId== movieId && e.Img==Img, cancellationToken: cancellationToken);
            if (MovieSubImgInDb is null)
                return RedirectToAction("NotFoundPage", "Home");

            //remove old image in wwwroot
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie-images", MovieSubImgInDb.Img);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }

            _movieSubImgRepository.Delete(MovieSubImgInDb);
            await _movieSubImgRepository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Edit), new { id = movieId });
        }



    }
}
