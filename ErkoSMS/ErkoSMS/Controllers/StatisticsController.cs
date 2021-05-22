using ErkoSMS.DataAccess;
using ErkoSMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ErkoSMS.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Statistics
        public ActionResult OrderStatistics()
        {
            return View();
        }


        public ActionResult CustomerHitList()
        {
            return View();
        }

        public ActionResult GetCustomerStatistics(DateTime startDate, DateTime endDate)
        {
            var customerStatistics = new List<CustomerHitListViewModel>();

            var orderDataService = new SalesDataService();
            var sales = orderDataService.GetSales(startDate, endDate);

            var customerSales = sales.GroupBy(x => x.Customer.Name).ToList();

            foreach (var customerSale in customerSales)
            {
                var customerStatistic = new CustomerHitListViewModel();
                customerStatistic.CustomerName = customerSale.Key;
                var completedOrders = customerSale.Where(x => x.SalesState == DataAccess.Model.SalesState.InvoiceDoneAndPacked).ToList();
                customerStatistic.TotalCompletedOrderNumber = completedOrders.Count();
                customerStatistic.TotalCompletedOrderIncomeTL = completedOrders.Where(x => x.Currency == DataAccess.Model.Currency.Tl).Sum(y => y.TotalPrice);
                customerStatistic.TotalCompletedOrderIncomeEuro = completedOrders.Where(x => x.Currency == DataAccess.Model.Currency.Eur).Sum(y => y.TotalPrice);
                customerStatistic.TotalCompletedOrderIncomeDollar = completedOrders.Where(x => x.Currency == DataAccess.Model.Currency.Usd).Sum(y => y.TotalPrice);

                var ongoingOrders = customerSale.Where(x => x.SalesState != DataAccess.Model.SalesState.InvoiceDoneAndPacked && x.SalesState != DataAccess.Model.SalesState.Rejected).ToList();
                customerStatistic.TotalOngoingOrderNumber = ongoingOrders.Count();
                customerStatistic.TotalOngoingOrderIncomeTL = ongoingOrders.Where(x => x.Currency == DataAccess.Model.Currency.Tl).Sum(y => y.TotalPrice);
                customerStatistic.TotalOngoingOrderIncomeEuro = ongoingOrders.Where(x => x.Currency == DataAccess.Model.Currency.Eur).Sum(y => y.TotalPrice);
                customerStatistic.TotalOngoingOrderIncomeDollar = ongoingOrders.Where(x => x.Currency == DataAccess.Model.Currency.Usd).Sum(y => y.TotalPrice);

                customerStatistics.Add(customerStatistic);
            }


            return new JsonResult()
            {
                Data = customerStatistics.OrderByDescending(x => x.TotalCompletedOrderIncomeTL).ToList(),
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }





        public ActionResult OrderStatusOverview()
        {
            return View();
        }

        public ActionResult ProductHitList()
        {
            return View();
        }

        public ActionResult SalesManStatistics()
        {
            return View();
        }


    }
}