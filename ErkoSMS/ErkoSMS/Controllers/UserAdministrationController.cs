using ErkoSMS.Models;
using ErkoSMS.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}