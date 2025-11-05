using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ASP.NETCore5.Models;
using ASP.NETCore5.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace ASP.NETCore5.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly UserManager<AppUser> _userManager;
        private readonly IMailService _mailService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IReCaptchaService _recaptchaService;
        public AccountController(UserManager<AppUser> userManager,
                                 IMailService mailService,
                                 SignInManager<AppUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 IReCaptchaService recaptchaService,
                                 IHttpClientFactory httpClientFactory
)
        {
            _userManager = userManager;
            _mailService = mailService;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _recaptchaService = recaptchaService;
            _httpClientFactory = httpClientFactory;

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

        //Login nhận jwt token lưu vào sesion gọi API /api/jwtauth/login2 để auth

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginView model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:44327");

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/jwtauth/login2", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                return View(model);
            }

            var resString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(resString);
            var root = doc.RootElement;

            string token = root.TryGetProperty("token", out var t) ? t.GetString() ?? "" : "";
            string username = root.TryGetProperty("username", out var u) ? u.GetString() ?? "" : "";
            string roles = "";

            if (root.TryGetProperty("roles", out var r))
            {
                if (r.ValueKind == JsonValueKind.Array)
                    roles = string.Join(",", r.EnumerateArray().Select(x => x.GetString()));
                else if (r.ValueKind == JsonValueKind.String)
                    roles = r.GetString();
            }

            // ✅ Lưu Session để Middleware & Layout dùng
            HttpContext.Session.SetString("JWToken", token);
            HttpContext.Session.SetString("UserName", username);
            HttpContext.Session.SetString("UserRoles", roles ?? "");
            await HttpContext.Session.CommitAsync();
            System.Diagnostics.Debug.WriteLine($"[Login] {username} - Roles={roles}");

            // ✅ Chuyển hướng về Home (hoặc Admin nếu có role Admin)
            if (roles == "Admin" || roles.Contains("Manager"))
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        // Login Logout với Identity
        //public IActionResult Login()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login([FromForm] LoginView model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        if (User.IsInRole("Admin"))
        //            return RedirectToAction("Index", "Admin");
        //        if (User.IsInRole("Manager"))
        //            return RedirectToAction("Dashboard", "Manager");

        //        return RedirectToAction("Index", "Home");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        AppUser user = await _userManager.FindByNameAsync(model.UserName);
        //        if (user != null)
        //        {
        //            await _signInManager.SignOutAsync();
        //            Microsoft.AspNetCore.Identity.SignInResult result =
        //                await _signInManager.PasswordSignInAsync(
        //                    user, model.Password, false, false);

        //            if (result.Succeeded)
        //            {
        //                return Redirect(returnUrl ?? "/account");
        //            }
        //            else
        //                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
        //        }
        //    }
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Logout()
        //{
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction("Login");
        //}




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

        public async Task<IActionResult> SendEmail()
        {
            // send to email
            MailRequest req = new MailRequest();
            req.ToEmail = "huynhphuc552002@gmail.com";
            req.Subject = "Test Email from ASP.NET Core 5 MVC";
            req.Body = "This is a simple content.";
            await _mailService.SendEmailAsync(req);

            return View();
        }

        public IActionResult Register2()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register2([FromForm] UserView model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.UserName,
                    FullName = model.FullName,
                    Email = model.Email
                };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    IdentityResult result2 = await _userManager.AddToRoleAsync(user, "Member");
                    if (!result2.Succeeded)
                    {
                        ModelState.AddModelError(nameof(LoginView.UserName), "Failed to create the user");
                        return View(model);
                    }

                    var usr = await _userManager.FindByEmailAsync(model.Email);
                    var userId = await _userManager.GetUserIdAsync(usr);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(usr);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    string link = Url.Action("ConfirmEmail",
                        "Account", new
                        {
                            userid = userId,
                            code = code
                        },
                        protocol: HttpContext.Request.Scheme);

                    // send to email
                    MailRequest req = new MailRequest();
                    req.ToEmail = usr.Email;
                    req.Subject = "Email Confirmation";
                    req.Body = $"<p>Click this link to activate your account: <href a='" + link + "'>" + link + "</a></p>";
                    await _mailService.SendEmailAsync(req);

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(nameof(LoginView.UserName), "Failed to create the user");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string code, string userid)
        {
            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var user = await _userManager.FindByIdAsync(userid);
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                ViewBag.Status = "Email confirmation was succeed";
            }
            else
            {
                ViewBag.Status = "Email confirmation was failed or invalid token / userid";
            }
            return View();
        }

        public IActionResult RegisterCaptcha()
        {
            ViewData["ReCaptchaKey"] = _recaptchaService.Configs.Key;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCaptcha([FromForm] UserView model)
        {
            ViewData["ReCaptchaKey"] = _recaptchaService.Configs.Key;

            if (ModelState.IsValid)
            {
                // validate reCAPTCHA
                string token = Request.Form["g-recaptcha-response"];
                if (!_recaptchaService.ValidateReCaptcha(token))
                {
                    ModelState.AddModelError(nameof(UserView.UserName),
                        "Bạn chưa xác nhận CAPTCHA hoặc xác nhận không hợp lệ.");
                    return View(model);
                }

                // tạo user mới
                var user = new AppUser
                {
                    UserName = model.UserName,
                    FullName = model.FullName,
                    Email = model.Email
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // gán role mặc định
                    IdentityResult result2 =  await _userManager.AddToRoleAsync(user, "Member");
                    if ( result2.Succeeded)
                    TempData["SuccessMessage"] = "Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.";
                    return RedirectToAction("Index");
                }

                // nếu có lỗi khi tạo user
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public IActionResult ForgotAccount()
        {
            return View(new ASP.NETCore5.Models.ForgotAccount());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotAccount([FromForm] ForgotAccount model)
        {
            if (!string.IsNullOrEmpty(model.Email))
            {
                AppUser user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // send to email
                    MailRequest req = new MailRequest();
                    req.ToEmail = user.Email;
                    req.Subject = "Forgot Account";
                    req.Body = "<p>Here is your account: " + user.UserName + "</p>";
                    await _mailService.SendEmailAsync(req);

                    return View("ForgotAccountConfirmation");
                }
            }
            return View(model);
        }

        public IActionResult ForgotAccountConfirmation()
        {
            return View();
        }
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new PasswordView());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([FromForm] PasswordView model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password == model.RetypedPassword)
                {
                    // Lấy user hiện tại đang đăng nhập
                    AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "User not found.");
                        return View(model);
                    }

                    // Kiểm tra mật khẩu hiện tại
                    bool isValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                    if (isValid)
                    {
                        IdentityResult result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);
                        if (result.Succeeded)
                        {
                            ViewBag.Message = "Password changed successfully!";
                            return View("ChangePasswordConfirmation");
                        }
                        else
                        {
                            foreach (var err in result.Errors)
                            {
                                ModelState.AddModelError("", err.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    }
                }
                else
                {
                    ModelState.AddModelError("RetypedPassword", "New passwords do not match.");
                }
            }

            return View(model);
        }

        public IActionResult ChangePasswordConfirmation()
        {
            return View();
        }
        public IActionResult ChangeEmail()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(ChangeEmailView model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                user.Email = model.NewEmail;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    ViewBag.Message = "Email changed successfully!";
                    return View("ChangeEmailConfirmation");
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public IActionResult ChangeEmailConfirmation()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View(new ASP.NETCore5.Models.ForgotAccount());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotAccount model)
        {
            if (!string.IsNullOrEmpty(model.UserName))
            {
                AppUser user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null && user.Email == model.Email)
                {
                    string token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    string link = Url.Action(
                        "ResetPassword",
                        "Account",
                        new { username = model.UserName, token = token },
                        protocol: HttpContext.Request.Scheme);

                    // Gửi email chứa link đặt lại mật khẩu
                    MailRequest req = new MailRequest
                    {
                        ToEmail = user.Email,
                        Subject = "Forgot Account",
                        Body = $"<p>Here is the link to reset your password:</p>" +
                               $"<p><a href='{link}'>Click here to reset password</a></p>"
                    };

                    await _mailService.SendEmailAsync(req);

                    ViewBag.Message = "Password reset link has been sent to your email.";
                    return View("ForgotPasswordConfirmation");
                }

                ModelState.AddModelError("", "Username or email is incorrect.");
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult ResetPassword(string username, string token)
        {
            var model = new ResetPasswordView
            {
                UserName = username,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordView model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            // Đặt lại mật khẩu bằng token
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (result.Succeeded)
            {
                ViewBag.Message = "Password has been reset successfully!";
                return RedirectToAction("ResetPasswordConfirmation");
            }
            else
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
            }

            return View(model);
        }
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
    }
}
