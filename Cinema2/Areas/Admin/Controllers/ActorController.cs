using System.Threading.Tasks;
using Cinema2.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cinema2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context;// = new();
        private readonly IRepository<Actor> _actorRepository;

        public ActorController(ApplicationDbContext context, IRepository<Actor> actorRepository)
        {
            _context = context;
            _actorRepository = actorRepository;
        }

        public async Task<IActionResult> Index(String Name ,CancellationToken cancellationToken, int page=1)
        {
            var actors =await _actorRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);

            if(Name is not null)
            {
                actors = actors.Where(e => e.Name.Contains(Name.Trim()));
                ViewBag.name = Name;
            }

            ViewBag.TotalPages = Math.Ceiling(actors.Count() / 5.0);
            ViewBag.currentPage = page;
            actors = actors.Skip((page - 1) * 5).Take(5);


            return View(actors.AsEnumerable());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Actor());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile img, CancellationToken cancellationToken)
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

                    actor.Img = fileName;
                }

                var actorCreated =await _actorRepository.AddAsync(actor, cancellationToken: cancellationToken);
                await _actorRepository.CommitAsync(cancellationToken);


                TempData["success-notification"] = "Add Actor Successfully";
                transaction.Commit();
            }
            catch (Exception ex)
            {
                TempData["error-notification"] = "Error While Saving Actor";
                transaction.Rollback();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var actor =await _actorRepository.GetOneAsync(e=>e.Id== id);
            if (actor is null)
                return RedirectToAction("NotFoundPage", "Home");
            return View(actor);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(Actor actor, IFormFile? img, CancellationToken cancellationToken)
        {
            var actorInDb =await _actorRepository.GetOneAsync(e=>e.Id == actor.Id , tracked: false);
            if (actorInDb is null)
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
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", actorInDb.Img);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                    //save image in db
                    actor.Img = fileName;
                }
            }
            else
            {
                actor.Img = actorInDb.Img;
            }

            _actorRepository.Update(actor);
            await _actorRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Edit Actor Successfully";
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (actor is null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            else
            {
                _actorRepository.Delete(actor);
                await _actorRepository.CommitAsync(cancellationToken);

                return RedirectToAction(nameof(Index));
            }
        }

    }
}
