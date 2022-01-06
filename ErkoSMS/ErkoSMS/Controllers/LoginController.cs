using ErkoSMS.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ErkoSMS.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Index(UserViewModel model)
        {
            bool.TryParse(ConfigurationManager.AppSettings["webpages:Validation"], out var expirationDisabled);
            if (expirationDisabled == false)
            {
                var expireDateString = HextoString(ConfigurationManager.AppSettings["webpages:Signature"]);
                var dateParts = expireDateString.Split('.').Select(x => Convert.ToInt32(x)).ToArray();
                var expireDate = new DateTime(dateParts[0], dateParts[1], dateParts[2]);
                if (DateTime.Now > expireDate)
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }

            }
            var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }
        private string HextoString(string InputText)
        {

            byte[] bb = Enumerable.Range(0, InputText.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(InputText.Substring(x, 2), 16))
                .ToArray();
            return System.Text.Encoding.ASCII.GetString(bb);
        }
    }
}