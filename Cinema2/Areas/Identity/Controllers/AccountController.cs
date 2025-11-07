using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Cinema2.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;


        //service => user manager - role manager
        //   Repo => User store  - role store
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender
          , IRepository<ApplicationUserOTP> applicationUserOTPRepository)
            
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationUserOTPRepository = applicationUserOTPRepository;
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            var user = new ApplicationUser()
            {
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                Email = registerVM.Email,
                UserName = registerVM.UserName
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Code);
                }
                return View(registerVM);
            }

            //send email confirmation

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", token, userId = user.Id }, Request.Scheme);  //34an yrg3 l nfs el domain

            await _emailSender.SendEmailAsync(registerVM.Email, "Ecommerce - Confirm Your Email!",
                $"<div><h1>Confirm Your Email By Clicking <a href='{link}'>Here</a> </h1></div>");

            await _userManager.AddToRoleAsync(user, SD.CUSTOMER_ROLE);

            return RedirectToAction("Login");
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                TempData["error-notification"] = "Invalid User";

            var result = await _userManager.ConfirmEmailAsync(user, token);  //token valid 24 hour

            if (!result.Succeeded)
                TempData["error-notification"] = "Invalid Or Expired Token";
            else
                TempData["success-notification"] = "Confirm Email Successfully";

            return RedirectToAction("Login");
        }


        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resendEmailConfirmationVM);
            }
            var user = await _userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name Or Email");
                return View(resendEmailConfirmationVM);
            }

            if (user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Already Confirmed");
                return View(resendEmailConfirmationVM);
            }

            //send email confirmation
            var token = _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", token, userId = user.Id }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email!, "Ecommerce -Resend Confirm Your Email!",
                $"<div><h1>Confirm Your Email By Clicking <a href='{link}'>Here</a> </h1></div>");

            return RedirectToAction("Login");
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(forgetPasswordVM);
            }
            var user = await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name Or Email");
                return View(forgetPasswordVM);
            }

            var userOTPs = await _applicationUserOTPRepository.GetAsync(e => e.ApplicationUserId == user.Id);
            var totalOTPs = userOTPs.Count(e => (DateTime.UtcNow - e.CreateAt).TotalHours < 24);

            if (totalOTPs > 3)
            {
                ModelState.AddModelError(string.Empty, "Too Many Attemps");
                return View(forgetPasswordVM);
            }

            var otp = new Random().Next(1000, 9999).ToString();

            await _applicationUserOTPRepository.AddAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationUserId = user.Id,
                CreateAt = DateTime.UtcNow,
                IsValid = true,
                OTP = otp,
                ValidTo = DateTime.UtcNow.AddDays(1)
            });
            await _applicationUserOTPRepository.CommitAsync();

            await _emailSender.SendEmailAsync(user.Email!, "Ecommerce - Reset Your Password",
                $"<div><h1>Use This OTP: {otp} To Reset Your Account. Don't Share It.</h1></div>");

            return RedirectToAction("ValidateOTP", new { userId = user.Id });
        }


        public IActionResult ValidateOTP(string userId)
        {
            return View(new ValidateOTPVM
            {
                ApplicationUserId = userId,
            });
        }

        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            var result = await _applicationUserOTPRepository.GetOneAsync(e => e.ApplicationUserId == validateOTPVM.ApplicationUserId &&
            e.OTP == validateOTPVM.OTP && e.IsValid);

            if (result is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid OTP");
                return RedirectToAction(nameof(validateOTPVM), new { userId = validateOTPVM.ApplicationUserId });
            }

            return RedirectToAction("NewPassword", new { userId = validateOTPVM.ApplicationUserId });
        }

        public IActionResult NewPassword(string userId)
        {
            return View(new NewPasswordVM
            {
                ApplicationUserId = userId,
            });
        }

        [HttpPost]
        public async Task<IActionResult> NewPassword(NewPasswordVM newPasswordVM)
        {
            var user = await _userManager.FindByIdAsync(newPasswordVM.ApplicationUserId);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name Or Email");
                return View(newPasswordVM);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPasswordVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Code);
                }
                return View(newPasswordVM);
            }

            return RedirectToAction("Login");
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name / Email or Password");
                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    ModelState.AddModelError(string.Empty, "too many attemps, try again after 5 min");
                else if (!user.EmailConfirmed)
                    ModelState.AddModelError(string.Empty, "please confirm your email first!!");
                else
                    ModelState.AddModelError(string.Empty, "Invalid User Name / Email or Password");

                return View(loginVM);
            }

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }



    }
}
