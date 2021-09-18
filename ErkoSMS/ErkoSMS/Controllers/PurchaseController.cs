using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.Model;
using ErkoSMS.Objects;
using ErkoSMS.ViewModels;
using Microsoft.AspNet.Identity;
using Excel = Microsoft.Office.Interop.Excel;

namespace ErkoSMS.Controllers
{
    public class PurchaseController : Controller
    {
        public ActionResult Index()
        {
            return GetAllPurchases();
        }

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
                TotalPrice = purchaseViewModel.TotalPrice,
                OrderId = 0,
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
        public int GetStockInformationByProductCode(string productCode)
        {
            var stock = new ORKADataService().GetStockByCode(productCode);
            return stock.RemainingAmount;
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
                TotalPrice = purchaseViewModel.UnitPrice * purchaseViewModel.Quantity,
                RequestedBySales = purchaseViewModel.RequestedBySales
            };
            if (purchaseViewModel.PurchaseState == PurchaseState.PurchaseSuccesful ||
                purchaseViewModel.PurchaseState == PurchaseState.PurchaseFailed)
            {
                purchase.PurchaseCloseDate = DateTime.Now;
            }

            if (purchaseViewModel.OrderId != 0)
            {
                var salesState = new SalesDataService().GetSalesById(purchaseViewModel.OrderId).SalesState;
                if (salesState == SalesState.PurchaseInProgress)
                {
                    if (purchaseViewModel.PurchaseState == PurchaseState.PurchaseSuccesful)
                    {
                        new SalesDataService().UpdateOrderState(purchaseViewModel.OrderId,
                            SalesState.PurchaseSuccesful);
                    }
                    else if (purchaseViewModel.PurchaseState == PurchaseState.PurchaseFailed)
                    {
                        new SalesDataService().UpdateOrderState(purchaseViewModel.OrderId,
                            SalesState.PurchaseFailed);
                    }
                }
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
            CalculateActiveTime(filteredPurchases);
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
            CalculateActiveTime(filteredPurchases);
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
            CalculateActiveTime(allPurchases);
            return new JsonResult()
            {
                Data = allPurchases,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase excelfile)
        {
            if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx") ||
                excelfile.FileName.EndsWith("csv"))
            {
                string path = Server.MapPath("~/Content/" + excelfile.FileName);
                excelfile.SaveAs(path);
                // Read data from excel file

                Excel.Application application = null;
                Excel.Workbook workbook = null;
                Excel.Worksheet worksheet = null;
                Excel.Range range = null;
                List<OrderLine> products = new List<OrderLine>();
                try
                {
                    application = new Excel.Application();
                    workbook = application.Workbooks.Open(path);
                    worksheet = workbook.ActiveSheet;
                    range = worksheet.UsedRange;

                    for (int i = 2; i <= range.Rows.Count; i++)
                    {
                        var product = new OrderLine();
                        for (int j = 1; j <= range.Columns.Count; j++)
                        {

                            //write the value to the console
                            if (range.Cells[i, j] != null && range.Cells[i, j].Value2 != null)
                            {
                                if (j == 1)
                                {
                                    product.ProductCode = range.Cells[i, j].Value2;
                                }
                                else if (j == 2)
                                {
                                    product.Quantity = (int)range.Cells[i, j].Value2;
                                }
                                else if (j == 3)
                                {
                                    product.UnitPrice = Convert.ToDouble(range.Cells[i, j].Value2.ToString());
                                }
                            }
                        }

                        product.StokQuantity = GetStockInformationByProductCode(product.ProductCode);
                        product.TotalPrice = product.UnitPrice * product.Quantity;
                        product.ProductDescription = GetProductDescriptionByProductCode(product.ProductCode);
                        products.Add(product);
                    }
                }
                finally
                {
                    workbook.Close(0);
                    application.Quit();

                    if (range != null) Marshal.ReleaseComObject(range);
                    if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                    if (workbook != null) Marshal.ReleaseComObject(workbook);
                    if (application != null) Marshal.ReleaseComObject(application);
                }
                return new JsonResult
                {
                    Data = products,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
            }
            return new JsonResult();
        }

        [HttpGet]
        public ActionResult DeletePurchase(int purchaseId)
        {
            var result = new PurchaseDataService().DeletePurchase(purchaseId);
            return new JsonResult()
            {
                Data = result,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public bool AssignPurchase(int purchaseId)
        {
            var purchaserUserGuid = User.Identity.GetUserId();

            new PurchaseDataService().AssignPurchase(purchaseId, purchaserUserGuid,PurchaseState.PurchaseInProgress);
            var orderId = new PurchaseDataService().GetAllPurchases().First(x => x.PurchaseId == purchaseId).OrderId;
            new SalesDataService().UpdateOrderState(orderId.Value, SalesState.PurchaseInProgress);
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

        private void CalculateActiveTime(IEnumerable<Purchase> purchases)
        {
            foreach (var purchase in purchases)
            {
                purchase.ActiveTime = purchase.PurchaseCloseDate.HasValue ?
                    purchase.PurchaseCloseDate.Value.Subtract(purchase.PurchaseStartDate).Days : 
                    DateTime.Now.Subtract(purchase.PurchaseStartDate).Days;
            }
        }
    }
}