using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.Model;
using ErkoSMS.ViewModels;
using Microsoft.AspNet.Identity;

namespace ErkoSMS.Controllers
{
    public class OrderController : Controller
    {
        public ActionResult Index()
        {
            return View(new OrderFilterParameters());
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

        public ActionResult CreateOrder()
        {
            FillViewBag();

            var orderViewModel = new OrderViewModel { OrderLines = new List<OrderLine>() };

            return View(orderViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder(OrderViewModel order)
        {
            var sales = new Sales();
            var salesDetails = new List<SalesDetail>();
            foreach (var orderLine in order.OrderLines)
            {
                salesDetails.Add(new SalesDetail { ProductCode = orderLine.ProductCode, Quantity = orderLine.Quantity, UnitPrice = orderLine.UnitPrice });
            }
            sales.Currency = order.Currency;
            sales.Customer = new CustomerDataService().GetCustomerById(order.CustomerId);
            sales.IsActive = order.IsActive;
            sales.SalesStartDate = DateTime.Now;
            sales.SalesUserName = User.Identity.Name;
            sales.SalesDetails = salesDetails;

            new SalesDataService().CreateOrder(sales);
            return new JsonResult();
            
        }

        public ActionResult ListOrder()
        {
            FillViewBag();
            return View(new OrderFilterParameters());
        }

        [HttpPost]
        public ActionResult GetFilteredSales(IEnumerable<int> customerIds, IEnumerable<SalesState> states,
                                            IEnumerable<Currency> currencies, string invoiceNo)
        {
            var salesByPerson = new SalesDataService().GetSalesBySalesPerson(User.Identity.GetUserId());

            var filteredSales = salesByPerson;
            if(customerIds != null)
            {
                filteredSales = salesByPerson.Where(x => customerIds.Contains(x.Customer.Id)).ToList();
            }
            if(states != null)
            {
                filteredSales = salesByPerson.Where(x => states.Contains(x.SalesState)).ToList();
            }
            if (currencies != null)
            {
                filteredSales = salesByPerson.Where(x => currencies.Contains(x.Currency)).ToList();
            }
            if(string.IsNullOrEmpty(invoiceNo) == false)
            {
                filteredSales = salesByPerson.Where(x => invoiceNo == x.InvoiceNumber).ToList();
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
        }
    }
}