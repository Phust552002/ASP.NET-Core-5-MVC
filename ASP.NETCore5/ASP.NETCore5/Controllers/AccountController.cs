using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ASP.NETCore5.Models;
using ASP.NETCore5.Services;
using System.Threading.Tasks;

namespace ASP.NETCore5.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        //private readonly IMailService _mailService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            //_mailService = mailService;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] UserView model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //if (!await _roleManager.RoleExistsAsync("User"))
                    //{
                    //    await _roleManager.CreateAsync(new IdentityRole("User"));
                    //}

                    //await _userManager.AddToRoleAsync(user, "User");

                    //await _mailService.SendEmailAsync(model.Email, "Welcome!", "Bạn đã đăng ký thành công tài khoản.");
                    //await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login([FromForm] LoginView model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
        //    }

        //    return View(model);
        //}
        public async Task<IActionResult> Login([FromForm] LoginView model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result =
                        await _signInManager.PasswordSignInAsync(
                            user, model.Password, false, false);

                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/account");
                    }
                    else
                        ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> GenerateRoles()
        {
            string[] roleNames = { "Admin", "Manager", "Member" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            return View();
        }

        public async Task<IActionResult> GenerateUsers()
        {
            AppUser userAva1 = await _userManager.FindByNameAsync("manager");
            if (userAva1 == null)
            {
                AppUser user1 = new AppUser
                {
                    UserName = "manager",
                    FullName = "Manager",
                    Email = "manager@email.com"
                };
                IdentityResult result = await _userManager.CreateAsync(user1, "Pass@Word1");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user1, "Manager");
                }
            }
            AppUser userAva2 = await _userManager.FindByEmailAsync("admin@email.com");
            if (userAva2 == null)
            {
                AppUser user1 = new AppUser
                {
                    UserName = "admin",
                    FullName = "Admin",
                    Email = "admin@email.com"
                };
                IdentityResult result = await _userManager.CreateAsync(user1, "Pass@Word1");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user1, "Admin");
                }
            }
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }






    }
}
