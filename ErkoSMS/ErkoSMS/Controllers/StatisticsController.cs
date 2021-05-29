using ErkoSMS.DataAccess;
using ErkoSMS.ViewModels;
using Microsoft.AspNet.Identity.Owin;
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
                Data = customerStatistics.OrderByDescending(x => (x.TotalCompletedOrderIncomeTL, x.TotalOngoingOrderIncomeTL)).ToList(),
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        public ActionResult GetSalesManStatistics(DateTime startDate, DateTime endDate)
        {
            var salesManStatistics = new List<SalesManStatisticViewModel>();

            var orderDataService = new SalesDataService();
            var sales = orderDataService.GetSales(startDate, endDate);

            var SalesOfSalesMan = sales.GroupBy(x => x.SalesUserGuid).ToList();

            foreach (var saleOfSaleMan in SalesOfSalesMan)
            {
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var salesManStatistic = new SalesManStatisticViewModel();
                salesManStatistic.SaleManName = userManager.Users.FirstOrDefault(x => x.Id == saleOfSaleMan.Key)?.UserName ?? "";
                var completedOrders = saleOfSaleMan.Where(x => x.SalesState == DataAccess.Model.SalesState.InvoiceDoneAndPacked).ToList();
                salesManStatistic.TotalCompletedOrderNumber = completedOrders.Count();
                salesManStatistic.TotalCompletedOrderIncomeTL = completedOrders.Where(x => x.Currency == DataAccess.Model.Currency.Tl).Sum(y => y.TotalPrice);
                salesManStatistic.TotalCompletedOrderIncomeEuro = completedOrders.Where(x => x.Currency == DataAccess.Model.Currency.Eur).Sum(y => y.TotalPrice);
                salesManStatistic.TotalCompletedOrderIncomeDollar = completedOrders.Where(x => x.Currency == DataAccess.Model.Currency.Usd).Sum(y => y.TotalPrice);

                var ongoingOrders = saleOfSaleMan.Where(x => x.SalesState != DataAccess.Model.SalesState.InvoiceDoneAndPacked && x.SalesState != DataAccess.Model.SalesState.Rejected).ToList();
                salesManStatistic.TotalOngoingOrderNumber = ongoingOrders.Count();
                salesManStatistic.TotalOngoingOrderIncomeTL = ongoingOrders.Where(x => x.Currency == DataAccess.Model.Currency.Tl).Sum(y => y.TotalPrice);
                salesManStatistic.TotalOngoingOrderIncomeEuro = ongoingOrders.Where(x => x.Currency == DataAccess.Model.Currency.Eur).Sum(y => y.TotalPrice);
                salesManStatistic.TotalOngoingOrderIncomeDollar = ongoingOrders.Where(x => x.Currency == DataAccess.Model.Currency.Usd).Sum(y => y.TotalPrice);

                salesManStatistics.Add(salesManStatistic);
            }


            return new JsonResult()
            {
                Data = salesManStatistics.OrderByDescending(x => (x.TotalCompletedOrderIncomeTL, x.TotalOngoingOrderIncomeTL)).ToList(),
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult GetProductStatistics(DateTime startDate, DateTime endDate)
        {
            var productStatistics = new List<ProductHitListViewModel>();

            var orderDataService = new SalesDataService();
            var sales = orderDataService.GetSales(startDate, endDate);


            var productSales = sales.SelectMany(sale => sale.SalesDetails.Select(saleDetail => new { sale, saleDetail })).GroupBy(y => y.saleDetail.ProductCode).ToList();


            foreach (var productSale in productSales)
            {
                var productStatistic = new ProductHitListViewModel();
                productStatistic.ProductName = productSale.Key;
                var completedOrders = productSale.Where(x => x.sale.SalesState == DataAccess.Model.SalesState.InvoiceDoneAndPacked).ToList();
                productStatistic.TotalCompletedOrderNumber = completedOrders.Count();
                productStatistic.TotalCompletedOrderIncomeTL = completedOrders.Where(x => x.sale.Currency == DataAccess.Model.Currency.Tl).Sum(y => y.saleDetail.Quantity * y.saleDetail.UnitPrice);
                productStatistic.TotalCompletedOrderIncomeEuro = completedOrders.Where(x => x.sale.Currency == DataAccess.Model.Currency.Eur).Sum(y => y.saleDetail.Quantity * y.saleDetail.UnitPrice);
                productStatistic.TotalCompletedOrderIncomeDollar = completedOrders.Where(x => x.sale.Currency == DataAccess.Model.Currency.Usd).Sum(y => y.saleDetail.Quantity * y.saleDetail.UnitPrice);

                var ongoingOrders = productSale.Where(x => x.sale.SalesState != DataAccess.Model.SalesState.InvoiceDoneAndPacked && x.sale.SalesState != DataAccess.Model.SalesState.Rejected).ToList();
                productStatistic.TotalOngoingOrderNumber = ongoingOrders.Count();
                productStatistic.TotalOngoingOrderIncomeTL = ongoingOrders.Where(x => x.sale.Currency == DataAccess.Model.Currency.Tl).Sum(y => y.saleDetail.Quantity * y.saleDetail.UnitPrice);
                productStatistic.TotalOngoingOrderIncomeEuro = ongoingOrders.Where(x => x.sale.Currency == DataAccess.Model.Currency.Eur).Sum(y => y.saleDetail.Quantity * y.saleDetail.UnitPrice);
                productStatistic.TotalOngoingOrderIncomeDollar = ongoingOrders.Where(x => x.sale.Currency == DataAccess.Model.Currency.Usd).Sum(y => y.saleDetail.Quantity * y.saleDetail.UnitPrice);

                productStatistics.Add(productStatistic);
            }


            return new JsonResult()
            {
                Data = productStatistics.OrderByDescending(x => (x.TotalCompletedOrderIncomeTL, x.TotalOngoingOrderIncomeTL)).ToList(),
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