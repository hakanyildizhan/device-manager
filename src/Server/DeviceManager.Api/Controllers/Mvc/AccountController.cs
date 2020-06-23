// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Service.Identity;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DeviceManager.Api.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityService _identityService;

        public AccountController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        // GET: Account
        public async Task<ActionResult> Index()
        {
            if (Request.IsAuthenticated)
            {
                bool adminLoggedIn = _identityService.IsUserInRole(User.Identity.GetUserId(), "Admin");
                if (adminLoggedIn)
                {
                    return RedirectToAction("Index", "Administration");
                }
            }

            bool adminUserPresent = await _identityService.IsAdminAccountPresentAsync();
            if (adminUserPresent)
            {
                return View("Login");
            }
            else
            {
                return View("Register");
            }
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool result = await _identityService.LoginAsync(model);
            return RedirectToAction("Index", "Administration");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            _identityService.Logout();
            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Register
        /// <summary>
        /// Registers a user with "Admin" role.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _identityService.RegisterWithRoleAsync(model, "Admin");
            return RedirectToAction("Index", "Home");
        }
    }
}