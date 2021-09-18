using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.SessionState;
using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.Model;
using ErkoSMS.Objects;
using ErkoSMS.ViewModels;
using Microsoft.AspNet.Identity;
using Excel = Microsoft.Office.Interop.Excel;

namespace ErkoSMS.Controllers
{
    public class OrderController : Controller
    {
        private static IList<OrderLine> _orderLine = new List<OrderLine>();
        public ActionResult Index()
        {
            return ListOrder();
        }

        public ActionResult OrderRow(OrderLine orderLine)
        {
            return PartialView(orderLine);
        }

        [HttpGet]
        public ActionResult GetAllSales()
        {
            var salesByPerson = new SalesDataService().GetSalesBySalesPerson(User.Identity.GetUserId());

            return new JsonResult()
            {
                Data = salesByPerson,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public int GetStockInformationByProductCode(string productCode)
        {
            var stock = new ORKADataService().GetStockByCode(productCode);
            var productId = new ProductDataService().GetProductByCode(productCode).Id;
            var reservedStock = new SalesDataService().GetReservedCountyProductId(productId);

            var usableStock = stock.RemainingAmount - reservedStock;
            return usableStock;
        }

        [HttpGet]
        public double GetLatestPriceForProductCode(string productCode)
        {
            return new ProductDataService().GetProductByCode(productCode).LastPrice;
        }

        [HttpGet]
        public string GetProductDescriptionByProductCode(string productCode)
        {
            var description = new ProductDataService().GetProductByCode(productCode).Description;
            return description;
        }

        public ActionResult CreateOrder()
        {
            FillViewBag();

            var orderViewModel = new OrderViewModel { OrderLines = new List<OrderLine>(), InvoiceDate = DateTime.Now };
            return View(orderViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder(OrderViewModel order)
        {
            var sales = new Sales();
            var salesDetails = new List<SalesDetail>();
            double totalPrice = 0;
            foreach (var orderLine in order.OrderLines)
            {
                totalPrice += orderLine.TotalPrice;
                salesDetails.Add(new SalesDetail { ProductCode = orderLine.ProductCode, Quantity = orderLine.Quantity, UnitPrice = orderLine.UnitPrice });
                var productId = new ProductDataService().GetProductByCode(orderLine.ProductCode).Id;
                new ProductDataService().UpdateProductLatestPrice(productId, orderLine.UnitPrice);
            }
            sales.Currency = order.Currency;
            sales.Customer = new CustomerDataService().GetCustomerById(order.CustomerId);
            sales.SalesStartDate = DateTime.Now;
            sales.SalesUserGuid = User.Identity.GetUserId();
            sales.SalesDetails = salesDetails;
            sales.TotalPrice = totalPrice;
            sales.InvoiceNumber = order.InvoiceNumber;
            sales.SalesState = order.State;
            sales.InvoiceDate = order.InvoiceDate;

            var isThereGap = order.OrderLines.Any(x => x.Quantity > x.StokQuantity);
            if (isThereGap)
            {
                sales.SalesState = SalesState.PurchaseRequested;
            }

            var orderId = new SalesDataService().CreateOrder(sales);
            var message = "Satış girişi başarıyla yapıldı.";
            foreach (var orderLine in order.OrderLines)
            {
                if (orderLine.Quantity > orderLine.StokQuantity)
                {
                    var gap = orderLine.Quantity - orderLine.StokQuantity;
                    var purchase = new Purchase
                    {
                        OrderId = orderId,
                        ProductCode = orderLine.ProductCode,
                        ProductId = new ProductDataService().GetProductByCode(orderLine.ProductCode).Id,
                        PurchaseStartDate = DateTime.Now,
                        PurchaseState = PurchaseState.PurchaseRequested,
                        RequestedBySales = true,
                        SalesUserName = User.Identity.Name,
                        Quantity = gap
                    };
                    new PurchaseDataService().CreatePurchase(purchase);
                    message +=
                        $"<br> <b>{orderLine.ProductCode}</b> kodlu ürün yeteri kadar stokta bulunmadığı için <b>{gap}</b> adet satın alma talebi yapıldı.";
                }
            }
            return Json(new AjaxResult(message));

        }

        public ActionResult ListOrder()
        {
            FillViewBag();
            return View(new OrderFilterParameters());
        }

        public ActionResult UpdateOrder(int orderId)
        {
            var salesDataService = new SalesDataService();
            var order = salesDataService.GetSalesById(orderId);
            foreach (var orderDetail in order.SalesDetails)
            {
                orderDetail.ProductDescription = GetProductDescriptionByProductCode(orderDetail.ProductCode);
            }
            FillViewBag();

            if (order.InvoiceDate.HasValue == false)
            {
                order.InvoiceDate = DateTime.MaxValue;
            }
            return PartialView(new OrderViewModel(order));
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult UpdateOrder(OrderViewModel order)
        {
            var salesDataService = new SalesDataService();
            var sales = new Sales
            {
                Id = order.OrderId,
                Customer = new CustomerDataService().GetCustomerById(order.CustomerId),
                InvoiceNumber = order.InvoiceNumber,
                SalesState = order.State,
                InvoiceDate = order.InvoiceDate,
                Currency = order.Currency,
                LastModifiedDate = DateTime.Now,
                SalesDetails = order.OrderLines?.Select(x => new SalesDetail()
                {
                    ProductCode = x.ProductCode,
                    Quantity = x.Quantity,
                    ProductDescription = x.ProductDescription,
                    SalesId = order.OrderId,
                    UnitPrice = x.UnitPrice
                })?.ToList() ?? new List<SalesDetail>(),
                TotalPrice = order.OrderLines?.Sum(x => x.TotalPrice) ?? 0
            };
            salesDataService.UpdateOrder(sales);
            return Json(new AjaxResult(true));
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

        [HttpPost]
        public ActionResult GetFilteredSales(IEnumerable<int> customerIds, IEnumerable<SalesState> states,
                                            IEnumerable<Currency> currencies, string invoiceNo)
        {
            var salesByPerson = new SalesDataService().GetSalesBySalesPerson(User.Identity.GetUserId());

            var filteredSales = salesByPerson;
            if (customerIds != null)
            {
                filteredSales = salesByPerson.Where(x => customerIds.Contains(x.Customer.Id)).ToList();
            }
            if (states != null)
            {
                filteredSales = filteredSales.Where(x => states.Contains(x.SalesState)).ToList();
            }
            if (currencies != null)
            {
                filteredSales = filteredSales.Where(x => currencies.Contains(x.Currency)).ToList();
            }
            if (string.IsNullOrEmpty(invoiceNo) == false)
            {
                filteredSales = filteredSales.Where(x => invoiceNo == x.InvoiceNumber).ToList();
            }
            return new JsonResult()
            {
                Data = filteredSales,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        private void FillViewBag()
        {
            var allCustomers = new CustomerDataService().GetAllCustomers().ToList();
            // Create a SelectListItem list from the alarm class configurations which are distinct by their names
            ViewBag.Customers =
                allCustomers.GroupBy(x => x.Id)
                    .Select(x => x.FirstOrDefault()).Select(
                        x =>
                            new SelectListItem
                            {
                                Text = x.Name,
                                Value = x.Id.ToString()
                            })
                    .ToList();

            ViewBag.SaleStates = EnumHelper.GetSelectList(typeof(SalesState));
            ViewBag.Currencies = EnumHelper.GetSelectList(typeof(Currency));


        }
    }
}