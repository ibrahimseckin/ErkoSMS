﻿using ErkoSMS.Enums;
using ErkoSMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ErkoSMS.DataAccess.Model;

namespace ErkoSMS.Controllers
{

    public class HomeController : Controller
    {

        LeftMenuViewModel AdministratorNavigations = new LeftMenuViewModel
        {
            Navigations = new List<LeftMenu>
            {
                    new LeftMenu(new LeftMenuItem("Kullanıcı Yönetimi", "UserAdministration", "Index"),null),
                    new LeftMenu(new LeftMenuItem("Ürünler", "Product", "Index"),null),
                    new LeftMenu(new LeftMenuItem("Müşteriler", "Customer", "Index"),null),
                    new LeftMenu(new LeftMenuItem("Satış Yönetimi", "", ""), 
                        new List<LeftMenuItem>{ new LeftMenuItem("Satışları Listele","Order","Index"),
                        new LeftMenuItem("Yeni Satış","Order","Index")}),
                    new LeftMenu(new LeftMenuItem("Stok Listesi", "Stock", "Index"),null)

            }
        };

        private LeftMenuViewModel SalesNavigations = new LeftMenuViewModel
        {
            Navigations = new List<LeftMenu>
            {
                  new LeftMenu(new LeftMenuItem("Satış Yönetimi", "", ""),
                        new List<LeftMenuItem>{ new LeftMenuItem("Satışları Listele","Order","Index"),
                        new LeftMenuItem("Yeni Satış","Order","Index")}),
                 new LeftMenu(new LeftMenuItem("Ürünler", "Product", "Index"),null)
            }
        };

        LeftMenuViewModel AnynomousNavigations = new LeftMenuViewModel
        {
            Navigations = new List<LeftMenu> {
                new LeftMenu(new LeftMenuItem("Home", "Home", "Index"),null),
                new LeftMenu(new LeftMenuItem("About", "Home", "About"),null),
                new LeftMenu(new LeftMenuItem("Contact", "Home", "Contact"),null),
            }
        };

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Role = User.IsInRole(UserTypes.Administrator.Name) ? UserTypes.Administrator.Name : "";
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
            if (User.IsInRole(UserTypes.Administrator.Name))
            {
                return AdministratorNavigations;
            }
            if (User.IsInRole(UserTypes.Accountant.Name))
            {
                return AnynomousNavigations;
            }
            if (User.IsInRole(UserTypes.Purchaser.Name))
            {
                return AnynomousNavigations;
            }
            if (User.IsInRole(UserTypes.SalesMan.Name))
            {
                return SalesNavigations;
            }
            if (User.IsInRole(UserTypes.WareHouseMan.Name))
            {
                return AnynomousNavigations;
            }

            return AnynomousNavigations;

        }




    }
}