using ErkoSMS.Enums;
using ErkoSMS.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.Model;
using ErkoSMS.Objects;
using Microsoft.Office.Interop.Excel;

namespace ErkoSMS.Controllers
{

    public class PackingController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
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

        [HttpGet]
        public ActionResult GetReadyForPackingOrders()
        {
            var allOrders = new SalesDataService().GetSalesByState(SalesState.WaitForPacking);
            var orderedOrders = allOrders.OrderBy(x => x.InvoiceDate);
            var allOrdersForView = orderedOrders.Select(x => new OrderViewModel(x));
            return new JsonResult()
            {
                Data = allOrdersForView,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public ActionResult GetPackedOrders()
        {
            var allOrders = new List<Sales>();
            var packingReadyOrders = new SalesDataService().GetSalesByState(SalesState.PackingIsReady);
            var packingReadyWaitingPaymentOrders = new SalesDataService().GetSalesByState(SalesState.PackingIsReadyAndWaitForPayment);

            allOrders.AddRange(packingReadyOrders);
            allOrders.AddRange(packingReadyWaitingPaymentOrders);

            var orderedOrders = allOrders.OrderBy(x => x.InvoiceDate);
            var allOrdersForView = orderedOrders.Select(x => new OrderViewModel(x));
            return new JsonResult()
            {
                Data = allOrdersForView,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public ActionResult PackOrder(int orderId)
        {
            var salesDataService = new SalesDataService();
            var order = salesDataService.GetSalesById(orderId);

            var packingDataService = new PackingDataService();
            var allPallets = packingDataService.GetAllPallets();

            ViewBag.Order = new OrderViewModel(order);
            ViewBag.Pallets = allPallets.Select(x=> new PalletViewModel(x)).ToList();

            return PartialView();
        }

        [HttpGet]
        public ActionResult PackProduct(string productCode)
        {
            ViewBag.ProductCodeToPack = productCode;
            var packingDataService = new PackingDataService();
            var allPallets = packingDataService.GetAllPallets();


            ViewBag.Pallets = allPallets.Select(x => new PalletViewModel(x)).ToList();

            return PartialView();
        }

        [HttpPost]
        public ActionResult SavePacking(string productCode)
        {

        }
    }
}