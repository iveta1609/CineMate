using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CineMate.Models; 

namespace CineMate.Controllers
{
    [AllowAnonymous]

    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly SignInManager<IdentityUser> _signInMgr;
        private readonly RoleManager<IdentityRole> _roleMgr;

        public AccountController(
            UserManager<IdentityUser> userMgr,
            SignInManager<IdentityUser> signInMgr,
            RoleManager<IdentityRole> roleMgr)
        {
            _userMgr = userMgr;
            _signInMgr = signInMgr;
            _roleMgr = roleMgr;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel vm, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(vm);

            var user = new IdentityUser { UserName = vm.Email, Email = vm.Email };
            var result = await _userMgr.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            if (!await _roleMgr.RoleExistsAsync("Client"))
                await _roleMgr.CreateAsync(new IdentityRole("Client"));

            var addToRoleRes = await _userMgr.AddToRoleAsync(user, "Client");
            if (!addToRoleRes.Succeeded)
            {
                foreach (var err in addToRoleRes.Errors)
                    ModelState.AddModelError("", err.Description);
                return View(vm);
            }

            await _signInMgr.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl ?? "/");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(vm);

            var result = await _signInMgr.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
                return LocalRedirect(returnUrl ?? "/");

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInMgr.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
