using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ASP.NETCore5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ASP.NETCore5.Controllers
{
    [Authorize(Roles = "Manager,Admin")]

    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult List()
        {
            return View(_userManager.Users);
        }

        public async Task<IActionResult> Details(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            return View(user);
        }
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("List");
        }
    }
}
