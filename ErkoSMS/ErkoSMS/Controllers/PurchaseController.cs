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
            var purchase = new PurchaseViewModel { };
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

        public ActionResult UpdatePurchase(int purchaseId)
        {
            var purchaseDataService = new PurchaseDataService();
            var purchase = purchaseDataService.GetAllPurchases().First(x => x.PurchaseId == purchaseId);
            FillViewBag();
            return PartialView(new PurchaseViewModel(purchase));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePurchase(PurchaseViewModel purchaseViewModel)
        {
            var PurchaseDataService = new PurchaseDataService();
            var purchase = new Purchase()
            {
                PurchaseState = purchaseViewModel.PurchaseState,
                SupplierId = purchaseViewModel.SupplierId,
                OrderId = purchaseViewModel.OrderId,
                ProductCode = purchaseViewModel.ProductCode,
                Quantity = purchaseViewModel.Quantity,
                SalesUserName = purchaseViewModel.SalesUserName,
                PurchaseStartDate = purchaseViewModel.PurchaseStartDate,
                UnitPrice = purchaseViewModel.UnitPrice,
                Currency = purchaseViewModel.Currency,
                ProductId = purchaseViewModel.ProductId,
                PurchaseId = purchaseViewModel.PurchaseId,
                PurchaserUserGuid = purchaseViewModel.PurchaserUser,
                TotalPrice = purchaseViewModel.TotalPrice,
                RequestedBySales = purchaseViewModel.RequestedBySales
            };
            if (purchaseViewModel.PurchaseState == PurchaseState.PurchaseSuccesful ||
                purchaseViewModel.PurchaseState == PurchaseState.PurchaseSuccesful)
            {
                purchase.PurchaseCloseDate = DateTime.Now;
            }

            var result = PurchaseDataService.UpdatePurchase(purchase);

            return Json(result ? new AjaxResult(true) : new AjaxResult(false));
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
        public ActionResult GetNonAssignedPurchases()
        {
            var allPurchases = new PurchaseDataService().GetAllPurchases();

            var filteredPurchases = allPurchases.Where(x => x.OrderId.HasValue)
                .Where(x => string.IsNullOrEmpty(x.PurchaserUserGuid));
            return new JsonResult()
            {
                Data = filteredPurchases,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult GetPurchasesAssignedToMe()
        {
            var allPurchases = new PurchaseDataService().GetAllPurchases();

            var filteredPurchases = allPurchases.Where(x => x.PurchaserUserGuid == User.Identity.GetUserId());
            return new JsonResult()
            {
                Data = filteredPurchases,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult GetAllPurchases()
        {
            var allPurchases = new PurchaseDataService().GetAllPurchases();
            return new JsonResult()
            {
                Data = allPurchases,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public bool AssignPurchase(int purchaseId)
        {
            var purchaserUserGuid = User.Identity.GetUserId();

            var result = new PurchaseDataService().AssignPurchase(purchaseId, purchaserUserGuid);
            return true;
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