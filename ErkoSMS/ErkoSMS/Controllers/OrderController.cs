using System;
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
    public class OrderController : Controller
    {
        public ActionResult Index()
        {
            return ListOrder();
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

        [HttpGet]
        public string GetProductDescriptionByProductCode(string productCode)
        {
            var description = new ProductDataService().GetProductByCode(productCode).Description;
            return description;
        }

        public ActionResult CreateOrder()
        {
            FillViewBag();

            var orderViewModel = new OrderViewModel {OrderLines = new List<OrderLine>(), InvoiceDate = DateTime.Now};
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
            return PartialView(new OrderViewModel(order));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOrder (OrderViewModel order)
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
                LastModifiedDate = DateTime.Now
            };
            salesDataService.UpdateOrder(sales);
            return Json(new AjaxResult(true));
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