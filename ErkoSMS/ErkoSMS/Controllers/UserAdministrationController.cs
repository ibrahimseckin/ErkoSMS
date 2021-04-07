using ErkoSMS.Enums;
using ErkoSMS.Models;
using ErkoSMS.Objects;
using ErkoSMS.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ErkoSMS.Controllers
{
    public class UserAdministrationController : Controller
    {
        // GET: UserAdministration
        public ActionResult Index()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var users = userManager.Users.Select(x => new UserViewModel
            {
                UserName = x.UserName,
                UserType = userManager.GetRolesAsync(x.Id).ConfigureAwait(false).GetAwaiter().GetResult().FirstOrDefault() ?? ""
            }).ToList();
            var userAdministrationViewModel = new UserAdministrationViewModel { Users = users };

            return View(userAdministrationViewModel);
        }

        public ActionResult CreateUser()
        {
            return PartialView();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(UserViewModel model)
        {
                var user = new ApplicationUser { UserName = model.UserName };
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, UserType.Administrator.ToString());
                    return Json(new AjaxResult(new { Code = AjaxResultCode.Success }));
                }
            else
            {
                return Json(new AjaxResult(AjaxResultCode.UserFailure,"Hata"));
            }
            
        }
    }
}