using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.Model;
using Microsoft.AspNet.Identity;

namespace ErkoSMS.Controllers
{
    public class OrderController : Controller
    {
        public ActionResult Index()
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
            var stock = 10;

            return new JsonResult()
            {
                Data = stock,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
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
    }
}