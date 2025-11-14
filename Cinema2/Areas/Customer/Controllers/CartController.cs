using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.Issuing;

namespace Cinema2.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IMovieRepository _movieRepository;

        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository, IRepository<Promotion> promotionRepository, IMovieRepository movieRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _promotionRepository = promotionRepository;
            _movieRepository = movieRepository;
        }

        public async Task<IActionResult> Index(string code)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            var cart = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Movie, e => e.applicationUser, e => e.Movie.category, e => e.Movie.ciinema]);

            var promotion = await _promotionRepository.GetOneAsync(e => e.Code == code && e.IsValid);

            if (promotion is not null)
            {
                var result = cart.FirstOrDefault(e => e.MovieId == promotion.MovieId);
                if (result is not null)
                {
                    result.Price -= result.Movie.TicketPrice * (promotion.Discount / 100);
                }

                await _cartRepository.CommitAsync();

            }
            return View(cart);
        }

        public async Task<IActionResult> AddToCart(int count, int movieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var movieInDb = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);

            if (movieInDb is not null)   //mawgood
            {
                movieInDb.Count += count;
                await _cartRepository.CommitAsync(cancellationToken);
                TempData["success-notification"] = "Update Movie Count In Cart Successfully";
                return RedirectToAction("Index", "Home");
            }

            await _cartRepository.AddAsync(new()
            {
                MovieId = movieId,
                ApplicationUserId = user.Id,
                Count = count,
                Price = (await _movieRepository.GetOneAsync(e => e.Id == movieId)).TicketPrice
            }, cancellationToken: cancellationToken);

            await _cartRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Add Movie To Cart Successfully";
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> IncrementMovie(int movieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var movie = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);
            if (movie is null)
            {
                return NotFound();
            }
            movie.Count += 1;
            await _cartRepository.CommitAsync(cancellationToken);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DecrementMovie(int movieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var movie = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);
            if (movie is null)
            {
                return NotFound();
            }
            if (movie.Count < 1)
            {
                _cartRepository.Delete(movie);
            }
            else
            {
                movie.Count -= 1;
            }
            await _cartRepository.CommitAsync(cancellationToken);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteMovie(int movieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var movie = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);
            if (movie is null)
            {
                return NotFound();
            }
            _cartRepository.Delete(movie);
            await _cartRepository.CommitAsync(cancellationToken);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return NotFound();
            }
            var cart = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Movie]);

            if (cart is null)
            {
                return NotFound();
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/cancel",
            };

            foreach (var item in cart)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Movie.Name,
                            Description = item.Movie.Description,
                        },
                        UnitAmount = (long)item.Price * 100
                    },
                    Quantity = item.Count,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }
    }
}

