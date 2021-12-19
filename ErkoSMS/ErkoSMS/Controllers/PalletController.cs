﻿using ErkoSMS.Enums;
using ErkoSMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.Model;
using ErkoSMS.Objects;

namespace ErkoSMS.Controllers
{

    public class PalletController : Controller
    {

        [HttpGet]
        public ActionResult ListPallets()
        {
            var allPallets = new PackingDataService().GetAllPallets();
            var orderedPallets = allPallets.OrderBy(x => x.Id);
            var allPalletsForView = orderedPallets.Select(x => new PalletViewModel(x));
            return new JsonResult()
            {
                Data = allPalletsForView,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreatePallet()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult CreatePallet(PalletViewModel pallet)
        {
            var packingDataService = new PackingDataService();
            var result = packingDataService.CreatePallet(pallet);
            if (result)
            {
                return Json(new AjaxResult("Sandık başarıyla kaydedildi."));
            }
            return Json(new AjaxResult("Sandık kaydedilemedi!"));
        }

        private void FillViewBag()
        {
            

        }

    }
}