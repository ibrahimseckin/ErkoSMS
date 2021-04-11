using ErkoSMS.Enums;
using ErkoSMS.Models;
using ErkoSMS.Objects;
using ErkoSMS.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ErkoSMS.Controllers
{
    public class UserAdministrationController : Controller
    {
        // GET: UserAdministration
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var users = userManager.Users.Select(x => new UserViewModel
            {
                Id = x.Id,
                UserName = x.UserName,
                UserType = userManager.GetRolesAsync(x.Id).ConfigureAwait(false).GetAwaiter().GetResult().FirstOrDefault() ?? "",
            }).ToList();
            var userAdministrationViewModel = new UserAdministrationViewModel { Users = users };


            return View(userAdministrationViewModel);
        }

        public ActionResult CreateUser()
        {
            return PartialView();
        }

        public ActionResult EditUser(string username)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.Users.FirstOrDefault(x => x.UserName.Equals(username));
            var userType = userManager.GetRolesAsync(user.Id).ConfigureAwait(false).GetAwaiter().GetResult().FirstOrDefault() ?? "";
            var userViewModel = new UserViewModel() { UserName = username, UserType = userType, Id = user.Id };
            return PartialView(userViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(UserViewModel model)
        {
            var user = new ApplicationUser { UserName = model.UserName };
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var result = await userManager.CreateAsync(user, model.Password ?? "");
            if (result.Succeeded)
            {
                userManager.AddToRole(user.Id, model.UserType);
                return Json(new AjaxResult(new { Code = AjaxResultCode.Success }));
            }
            else
            {
                var errorStringBuilder = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    errorStringBuilder.Append(error);
                }
                return Json(new AjaxResult(AjaxResultCode.UserFailure, errorStringBuilder.ToString()));
            }

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(UserViewModel model)
        {

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.Users.FirstOrDefault(x => x.Id.Equals(model.Id));

            var userType = userManager.GetRoles(user.Id).FirstOrDefault() ?? "";
            if (user != null)
            {
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var hashedPassowrd = userManager.PasswordHasher.HashPassword(model.Password);
                    user.PasswordHash = hashedPassowrd;
                }
                user.UserName = model.UserName;

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (model.UserType != userType)
                    {
                        userManager.ClearUserRoles(user.Id);
                        userManager.AddToRole(user.Id, model.UserType);
                    }
                    return Json(new AjaxResult(new { Code = AjaxResultCode.Success }));
                }
                else
                {
                    var errorStringBuilder = new StringBuilder();
                    foreach (var error in result.Errors)
                    {
                        errorStringBuilder.Append(error);
                    }
                    return Json(new AjaxResult(AjaxResultCode.UserFailure, errorStringBuilder.ToString()));
                }
            }
            return Json(new AjaxResult(AjaxResultCode.UserFailure, "Kullanıcı bulunamadı"));

        }


        [HttpPost]
        public ActionResult DeleteUser(string userName)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.Users.FirstOrDefault(x => x.UserName.Equals(userName));
            userManager.Delete(user);
            return Json(new AjaxResult(new { Code = AjaxResultCode.Success }));
        }

    }
}