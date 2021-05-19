﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.Model;
using ErkoSMS.Objects;
using ErkoSMS.ViewModels;
using Microsoft.AspNet.Identity;

namespace ErkoSMS.Controllers
{
    public class SupplierController : Controller
    {
        public ActionResult Index()
        {
            return ListSuppliers();
        }


        public ActionResult ListSuppliers()
        {

            return View();
        }

        [HttpGet]
        public ActionResult GetAllSuppliers()
        {
            var allSuppliers = new SupplierDataService().GetAllSuppliers();

            return new JsonResult()
            {
                Data = allSuppliers,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult CreateSupplier()
        {
            return View(new SupplierViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSupplier(SupplierViewModel supplierViewModel)
        {
            var supplier = new Supplier
            {
                Name = supplierViewModel.Name,
                Address = supplierViewModel.Address,
                Country = supplierViewModel.Country,
                PhoneNumber = supplierViewModel.PhoneNumber
            };

            new SupplierDataService().CreateSupplier(supplier);
            return Json(new AjaxResult(true));

        }

    }
}