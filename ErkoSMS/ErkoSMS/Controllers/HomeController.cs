using ErkoSMS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ErkoSMS.Controllers
{

    public class HomeController : Controller
    {
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
            ViewBag.Role = GetUserRole();
            ViewBag.SelectedMenu = selectedMenu;
            return PartialView("_LeftMenu");
        }

        private string GetUserRole()
        {
            if (User.IsInRole(UserType.Administrator.ToString()))
            {
                return UserType.Administrator.ToString();
            }
            if (User.IsInRole(UserType.Accountant.ToString()))
            {
                return UserType.Accountant.ToString();
            }
            if (User.IsInRole(UserType.Purchaser.ToString()))
            {
                return UserType.Purchaser.ToString();
            }
            if (User.IsInRole(UserType.SalesMan.ToString()))
            {
                return UserType.SalesMan.ToString();
            }
            if (User.IsInRole(UserType.WareHouseMan.ToString()))
            {
                return UserType.WareHouseMan.ToString();
            }

            return "";

        }




    }
}