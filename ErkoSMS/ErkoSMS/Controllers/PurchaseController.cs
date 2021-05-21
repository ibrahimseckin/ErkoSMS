using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.Model;
using ErkoSMS.Objects;
using ErkoSMS.ViewModels;
using Microsoft.AspNet.Identity;

namespace ErkoSMS.Controllers
{
    public class PurchaseController : Controller
    {
        public ActionResult CreatePurchase()
        {
            FillViewBag();
            var purchase = new PurchaseViewModel {};
            return View(purchase);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePurchase(PurchaseViewModel purchaseViewModel)
        {
            var purchase = new Purchase
            {
                PurchaserUserGuid = User.Identity.GetUserId(),
                SupplierId = purchaseViewModel.SupplierId,
                ProductId = new ProductDataService().GetProductByCode(purchaseViewModel.ProductCode).Id,
                ProductCode = purchaseViewModel.ProductCode,
                Currency = purchaseViewModel.Currency,
                PurchaseStartDate = DateTime.Now,
                PurchaseState = purchaseViewModel.PurchaseState,
                Quantity = purchaseViewModel.Quantity,
                RequestedBySales = false,
                UnitPrice = purchaseViewModel.UnitPrice,
                TotalPrice = purchaseViewModel.TotalPrice
            };

            new PurchaseDataService().CreatePurchase(purchase);
            return Json(new AjaxResult(true));

        }

        [HttpGet]
        public string GetProductDescriptionByProductCode(string productCode)
        {
            var description = new ProductDataService().GetProductByCode(productCode).Description;
            return description;
        }

        [HttpGet]
        public ActionResult GetStockInformationByProductCode(string productCode)
        {
            var stock = new ORKADataService().GetStockByCode(productCode);

            return new JsonResult()
            {
                Data = stock,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ListPurchase()
        {
            FillViewBag();
            return View(new PurchaseFilterParameters());
        }

        [HttpPost]
        public ActionResult GetFilteredPurchases(int? supplierId, PurchaseState? state)
        {
            var allPurchases = new PurchaseDataService().GetAllPurchases();

            var filteredPurchases = allPurchases;
            if (supplierId != 0)
            {
                filteredPurchases = filteredPurchases.Where(x => x.SupplierId == supplierId).ToList();
            }

            if (state != null)
            {
                filteredPurchases = filteredPurchases.Where(x => x.PurchaseState == state).ToList();
            }
            return new JsonResult()
            {
                Data = filteredPurchases,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult GetPurchasesRequestedByOrders()
        {
            var allPurchases = new PurchaseDataService().GetAllPurchases();

            var filteredPurchases = allPurchases.Where(x => x.OrderId.HasValue);
            return new JsonResult()
            {
                Data = filteredPurchases,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        private void FillViewBag()
        {
            var suppliers = new SupplierDataService().GetAllSuppliers().ToList();
            // Create a SelectListItem list from the alarm class configurations which are distinct by their names
            ViewBag.Suppliers =
                suppliers.GroupBy(x => x.SupplierId)
                    .Select(x => x.FirstOrDefault()).Select(
                        x =>
                            new SelectListItem
                            {
                                Text = x.Name,
                                Value = x.SupplierId.ToString(),
                                Selected = false
                            })
                    .ToList();
        }
    }
}