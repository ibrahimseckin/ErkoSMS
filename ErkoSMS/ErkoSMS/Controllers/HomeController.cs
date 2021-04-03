using ErkoSMS.Enums;
using ErkoSMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ErkoSMS.Controllers
{

    public class HomeController : Controller
    {

        LeftMenuViewModel AdministratorNavigations = new LeftMenuViewModel
        {
            Navigations = new List<LeftMenuItem>
            {
                    new LeftMenuItem("User Administration", "UserAdministration", "Index"),

            }
        };

        LeftMenuViewModel AnynomousNavigations = new LeftMenuViewModel
        {
            Navigations = new List<LeftMenuItem> {
                                                        new LeftMenuItem("Home", "Home", "Index"),
                                                        new LeftMenuItem("About", "Home", "About"),
                                                        new LeftMenuItem("Contact", "Home", "Contact"),
                                                    }
        };

        public ActionResult Index()
        {
            ViewBag.Role = User.IsInRole(UserType.Administrator.ToString()) ? UserType.Administrator.ToString() : "";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult LeftMenu(string selectedMenu)
        {
            var userNavigations = GetUserNavigation();
            ViewBag.SelectedMenu = selectedMenu;
            return PartialView("_LeftMenu", userNavigations);
        }

        private LeftMenuViewModel GetUserNavigation()
        {
            if (User.IsInRole(UserType.Administrator.ToString()))
            {
                return AdministratorNavigations;
            }
            if (User.IsInRole(UserType.Accountant.ToString()))
            {
                return AnynomousNavigations;
            }
            if (User.IsInRole(UserType.Purchaser.ToString()))
            {
                return AnynomousNavigations;
            }
            if (User.IsInRole(UserType.SalesMan.ToString()))
            {
                return AnynomousNavigations;
            }
            if (User.IsInRole(UserType.WareHouseMan.ToString()))
            {
                return AnynomousNavigations;
            }

            return AnynomousNavigations;

        }




    }
}