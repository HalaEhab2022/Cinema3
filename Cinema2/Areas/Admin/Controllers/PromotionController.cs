using Microsoft.AspNetCore.Mvc;

namespace Cinema2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PromotionController : Controller
    {
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IMovieRepository _movieRepository; //= new();

        public PromotionController(IRepository<Promotion> promotionRepository, IMovieRepository movieRepository)
        {
            _promotionRepository = promotionRepository;
            _movieRepository = movieRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var Promotions = await _promotionRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            return View(Promotions);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            ViewBag.movies = await _movieRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            return View(new Promotion());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Promotion promotion, CancellationToken cancellationToken)

        {
            //ModelState.Remove("img");

            if (!ModelState.IsValid)
            {
                ViewBag.movies = await _movieRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
                return View(promotion);
            }

            await _promotionRepository.AddAsync(promotion, cancellationToken);
            await _promotionRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Add Promotion Successfully";
            return RedirectToAction(nameof(Create));
        }


    }
}
