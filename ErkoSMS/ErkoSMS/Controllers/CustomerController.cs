using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.Model;
using ErkoSMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ErkoSMS.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetCustomers()
        {
            var customers = new CustomerDataService().GetAllCustomers();
            return new JsonResult()
            {
                Data = customers,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public ActionResult GetCustomer(string customerName)
        {
            var customers = new CustomerDataService().GetCustomerByNameWithWildCard(customerName);
            return new JsonResult()
            {
                Data = customers,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            };
        }


        [HttpGet]
        public ActionResult DeleteCustomer(int customerId)
        {
            var result = new CustomerDataService().DeleteCustomerById(customerId);
            return new JsonResult()
            {
                Data = result,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        public ActionResult CreateCustomer()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCustomer(CustomerViewModel customer)
        {
            var customerDataService = new CustomerDataService();
            customerDataService.CreateCustomer(customer);
            return new JsonResult()
            {
                Data = customer,
                ContentType = "application/json"
            };
        }

        public ActionResult EditCustomer(int customerId)
        {
            var customerDataService = new CustomerDataService();
            var customer = customerDataService.GetCustomerById(customerId);
            return PartialView(new CustomerViewModel(customer));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomer(CustomerViewModel customer)
        {
            var customerDataService = new CustomerDataService();
            var result = customerDataService.UpdateCustomer(customer);
            return new JsonResult()
            {
                Data = customer,
                ContentType = "application/json"
            };
        }
    }
}