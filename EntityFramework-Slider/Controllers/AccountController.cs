using EntityFramework_Slider.Models;
using EntityFramework_Slider.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EntityFramework_Slider.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            AppUser newUser = new()
            {
                UserName= model.UserName,
                Email= model.Email,
                FullName= model.FullName,
            };

            IdentityResult result = await _userManager.CreateAsync(newUser,model.Password);


            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
                return View(model);
            }

            await _signInManager.SignInAsync(newUser, false);

            return RedirectToAction(nameof(Login));
         
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            AppUser user = await _userManager.FindByEmailAsync(model.EmailorUsername);

            if (user is null) 
            { 
                user = await _userManager.FindByNameAsync(model.EmailorUsername);
            }

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "email or password is wrong");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "email or password is wrong");
                return View(model);
            }


            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
