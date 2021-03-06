using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.SessionState;
using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.DataService;
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
        public ActionResult GetAllOrders()
        {
            var allOrders = new SalesDataService().GetAllSales();
            var orderedOrdersByTime = allOrders.OrderByDescending(x => x.SalesStartDate);
            var orderViewModel = orderedOrdersByTime.Select(x => new OrderViewModel(x));
            return new JsonResult()
            {
                Data = orderViewModel,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public ActionResult GetAllActiveOrders()
        {
            var allActiveOrders = new SalesDataService().GetAllSales().Where(x => x.SalesState != SalesState.InvoiceDoneAndPacked);
            var orderedActiveOrders = allActiveOrders.OrderByDescending(x => x.SalesStartDate);
            var orderViewModel = orderedActiveOrders.Select(x => new OrderViewModel(x));
            return new JsonResult()
            {
                Data = orderViewModel,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public ActionResult GetMyOrders()
        {
            var myOrders = new SalesDataService().GetSalesBySalesPerson(User.Identity.GetUserId());
            var myOrderedOrders = myOrders.OrderByDescending(x => x.SalesStartDate);
            var orderViewModel = myOrderedOrders.Select(x => new OrderViewModel(x));
            return new JsonResult()
            {
                Data = orderViewModel,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public ActionResult GetMyActiveOrders()
        {
            var myOrders = new SalesDataService().GetSalesBySalesPerson(User.Identity.GetUserId());
            var myActiveOrders = myOrders.Where(x => x.SalesState != SalesState.InvoiceDoneAndPacked);
            var myOrderedActiveOrders = myActiveOrders.OrderByDescending(x => x.SalesStartDate);
            var orderViewModel = myOrderedActiveOrders.Select(x => new OrderViewModel(x));
            return new JsonResult()
            {
                Data = orderViewModel,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public int GetStockInformationByProductCode(string productCode)
        {
            var stock = new StockDataService().GetStockByCodeFromOrka(productCode);
            var productId = new ProductDataService().GetProductByCode(productCode).Id;
            var reservedStock = new SalesDataService().GetReservedCountyProductId(productId);

            var usableStock = stock.StockAmount - reservedStock;
            return usableStock;
        }

        [HttpGet]
        public double GetLatestPriceForProductCode(string productCode)
        {
            return new ProductDataService().GetProductByCode(productCode).LastPrice;
        }

        [HttpGet]
        public (string Description, string EnglishDescription) GetProductDescriptionByProductCode(string productCode)
        {
            var product = new ProductDataService().GetProductByCode(productCode);
            return (product.Description, product.EnglishDescription);
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
            sales.ExchangeRate = order.ExchangeRate;
            sales.Customer = new CustomerDataService().GetCustomerById(order.Customer.Id);
            sales.SalesStartDate = DateTime.Now;
            sales.SalesUserGuid = User.Identity.GetUserId();
            sales.SalesDetails = salesDetails;
            sales.TotalPrice = totalPrice;
            sales.InvoiceNumber = order.InvoiceNumber;
            sales.SalesState = order.State;
            sales.InvoiceDate = order.InvoiceDate;
            sales.Exporter = order.Exporter;
            sales.Comment = $"{User.Identity.Name}({DateTime.Now}):{order.Comment}";
            sales.DeliveryType = order.DeliveryType;
            sales.PaymentType = order.PaymentType;
            sales.TransportCost = order.TransportCost;

            var isThereGap = order.OrderLines.Any(x => x.Quantity > x.StokQuantity);
            if (isThereGap)
            {
                sales.SalesState = SalesState.PurchaseRequested;
            }

            var orderId = new SalesDataService().CreateOrder(sales);
            var message = "Sat???? giri??i ba??ar??yla yap??ld??.";
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
                        $"<br> <b>{orderLine.ProductCode}</b> kodlu ??r??n yeteri kadar stokta bulunmad?????? i??in <b>{gap}</b> adet sat??n alma talebi yap??ld??.";
                }
            }

            var uploadedFilesPath = Path.Combine(Server.MapPath("~"), "UploadedFiles");
            foreach (var file in Directory.GetFiles(uploadedFilesPath, $"{orderId}_*"))
            {
                System.IO.File.Delete(file);
            }

            if (order.Document != null)
            {
                order.Document.SaveAs(Path.Combine(uploadedFilesPath, $"{orderId}_{order.Document.FileName}"));
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
                var (description, englishDescription) = GetProductDescriptionByProductCode(orderDetail.ProductCode);
                orderDetail.ProductDescription = description;
                orderDetail.ProductEnglishDescription = englishDescription;
            }
            FillViewBag();

            if (order.InvoiceDate.HasValue == false)
            {
                order.InvoiceDate = DateTime.MaxValue;
            }

            var uploadedFilesPath = Path.Combine(Server.MapPath("~"), "UploadedFiles");
            var orderViewModel = new OrderViewModel(order);
            var fileName = Directory.GetFiles(uploadedFilesPath, $"{orderId}_*").FirstOrDefault();
            orderViewModel.AttachedDocumentFileName = Path.GetFileName(fileName);



            return PartialView(orderViewModel);
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult UpdateOrder(OrderViewModel order)
        {
            var salesDataService = new SalesDataService();
            var previousComment = salesDataService.GetSalesById(order.OrderId).Comment;
            int differentIndex = FindTheIndexOfFirstDifferentCharacter(previousComment, order.Comment);
            var sales = new Sales
            {
                Id = order.OrderId,
                Customer = new CustomerDataService().GetCustomerById(order.Customer.Id),
                Exporter = order.Exporter,
                InvoiceNumber = order.InvoiceNumber,
                SalesState = order.State,
                InvoiceDate = order.InvoiceDate,
                Currency = order.Currency,
                ExchangeRate = order.ExchangeRate,
                LastModifiedDate = DateTime.Now,
                SalesDetails = order.OrderLines?.Select(x => new SalesDetail()
                {
                    ProductCode = x.ProductCode,
                    Quantity = x.Quantity,
                    ProductDescription = x.ProductDescription,
                    ProductEnglishDescription = x.ProductEnglishDescription,
                    SalesId = order.OrderId,
                    UnitPrice = x.UnitPrice
                })?.ToList() ?? new List<SalesDetail>(),
                TotalPrice = order.OrderLines?.Sum(x => x.TotalPrice) ?? 0,
                Comment = order.Comment.Insert(differentIndex,$"\n{User.Identity.Name}({DateTime.Now}):"),
                DeliveryType = order.DeliveryType,
                PaymentType = order.PaymentType,
                TransportCost = order.TransportCost,
            };

            var existingQuantitiesbyProductCode = new Dictionary<string, int>();
            var orderDetails = salesDataService.GetSalesById(sales.Id).SalesDetails;
            foreach (var detail in orderDetails)
            {
                existingQuantitiesbyProductCode[detail.ProductCode] = detail.Quantity;
            }

            salesDataService.UpdateOrder(sales);



            var message = "Sat???? g??ncelleme ba??ar??yla yap??ld??.";
            //daha ??nceden eklenmi?? ??r??nlerde ??nceki miktar + stok toplam??na bak??l??r
            //yeni eklenen ??r??nlerde direk eklenmek istenen miktar ile stok kar????la??t??r??l??r
            foreach (var orderLine in order.OrderLines.Where(x => (x.IsAlreadyAddedProduct && x.Quantity > existingQuantitiesbyProductCode[x.ProductCode] + x.StokQuantity) ||
                                                                 (x.IsAlreadyAddedProduct == false && x.Quantity > x.StokQuantity)))
            {
                var gap = 0;
                //daha ??nce eklenen bir ??r??n ise daha ??nceki sipari?? miktar?? ve stok beraber ele al??nmal??
                if (orderLine.IsAlreadyAddedProduct)
                {
                    //stok negatif olmad??ysa stok adedi kullan??labilir
                    if (orderLine.StokQuantity >= 0)
                    {
                        gap = orderLine.Quantity - (orderLine.StokQuantity +
                                                    existingQuantitiesbyProductCode[orderLine.ProductCode]);
                    }
                    //stok negatif olduysa daha ??nce sat??n al??nma talebi yap??ld?????? i??in ??uanki sipari?? mikar?? ile bir ??ncekini kar????la??t??rmak sat??n alma talebi i??in yeterli
                    else
                    {
                        gap = orderLine.Quantity - existingQuantitiesbyProductCode[orderLine.ProductCode];
                    }
                }
                else
                {
                    gap = orderLine.Quantity - orderLine.StokQuantity;
                }


                //yeni bir ??r??n eklendi??inde di??er ??r??nler i??in gereksiz sat??n alma talebi yarat??lmas??n?? engellemek i??in if kontrol?? yap??ld??.
                if (existingQuantitiesbyProductCode.ContainsKey(orderLine.ProductCode) &&
                    existingQuantitiesbyProductCode[orderLine.ProductCode] != orderLine.Quantity)
                {
                    var purchase = new Purchase
                    {
                        OrderId = order.OrderId,
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
                        $"<br> <b>{orderLine.ProductCode}</b> kodlu ??r??n yeteri kadar stokta bulunmad?????? i??in <b>{gap}</b> adet sat??n alma talebi yap??ld??.";
                }

            }


            var uploadedFilesPath = Path.Combine(Server.MapPath("~"), "UploadedFiles");
            foreach (var file in Directory.GetFiles(uploadedFilesPath, $"{order.OrderId}_*"))
            {
                System.IO.File.Delete(file);
            }

            if (order.Document != null)
            {
                order.Document.SaveAs(Path.Combine(uploadedFilesPath, $"{order.OrderId}_{order.Document.FileName}"));
            }
            return Json(new AjaxResult(message));
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
                        var (description, englishDescription) = GetProductDescriptionByProductCode(product.ProductCode);
                        product.ProductDescription = description;
                        product.ProductEnglishDescription = englishDescription;
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
                Data = filteredSales.Select(x => new OrderViewModel(x)),
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public ActionResult DeleteOrder(int orderId)
        {
            var result = new SalesDataService().DeleteSales(orderId);
            return new JsonResult()
            {
                Data = result,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        [HttpGet]
        public ActionResult DeleteDocument(int orderId)
        {
            var uploadedFilesPath = Path.Combine(Server.MapPath("~"), "UploadedFiles");
            foreach (var file in Directory.GetFiles(uploadedFilesPath, $"{orderId}_*"))
            {
                System.IO.File.Delete(file);
            }

            return new JsonResult()
            {
                Data = true,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }


        [HttpGet]
        public FileResult DownloadDocument(int orderId)
        {
            var uploadedFilesPath = Path.Combine(Server.MapPath("~"), "UploadedFiles");
            var fileName = Directory.GetFiles(uploadedFilesPath, $"{orderId}_*").FirstOrDefault();
            string contentType = "application/pdf";
            return File(fileName, contentType, Path.GetFileName(fileName));
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

            var allExporters = new ExporterDataService().GetAllExporters().ToList();
            ViewBag.Exporters =
                allExporters.GroupBy(x => x.Id)
                    .Select(x => x.FirstOrDefault()).Select(
                        x =>
                            new SelectListItem
                            {
                                Text = x.Name,
                                Value = x.Id.ToString()
                            })
                    .ToList();
        }

        private int FindTheIndexOfFirstDifferentCharacter(string previousComment, string actualComment)
        {
            int length = Math.Min(previousComment.Length, actualComment.Length);

            int index = 0;
            for (index = 0; index < length; index++)
            {
                if (previousComment[index] != actualComment[index])
                {
                    return index;
                }
            }

            return index + 1;
        }
    }
}