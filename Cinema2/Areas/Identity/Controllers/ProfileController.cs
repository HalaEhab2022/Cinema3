using System.Threading.Tasks;
using Cinema2.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinema2.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return NotFound();
            }

            var userVM = user.Adapt<ApplicationUserVM>();
            return View(userVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return NotFound();
            }

            var names = applicationUserVM.FullName.Split(" ");

            user.PhoneNumber = applicationUserVM.PhoneNumber;
            user.Address = applicationUserVM.Address;
            user.FirstName = names[0];
            user.LastName = names[1];

            await _userManager.UpdateAsync(user);

            TempData["success-notification"] = "Update Profile Successfully";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> UpdatePassword(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return NotFound();
            }

            if (applicationUserVM.CurrentPassword is null || applicationUserVM.NewPassword is null)
            {
                TempData["error-notification"] = "Must have a CurrentPassword & NewPassword value";
                return RedirectToAction("Index");
            }

            var result = await _userManager.ChangePasswordAsync(user, applicationUserVM.CurrentPassword!, applicationUserVM.NewPassword!);

            if (!result.Succeeded)
            {
                TempData["error-notification"] = string.Join(" , ", result.Errors.Select(e => e.Code));

                return RedirectToAction("Index");
            }

            TempData["success-notification"] = "Update Profile Successfully";
            return RedirectToAction("Index");
        }

    }
}
