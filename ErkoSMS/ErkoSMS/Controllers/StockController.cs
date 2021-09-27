using ErkoSMS.DataAccess;
using ErkoSMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ErkoSMS.Controllers
{
    public class StockController : Controller
    {
        // GET: Stock
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult GetAllStocks()
        {
            var orkaDataService = new ORKADataService();
            var allStocks = orkaDataService.GetAllStocks().Select(x => new StockViewModel { Code = x.Code, Price = x.Price, RemainingAmount = x.RemainingAmount }).ToList();
            var reservedProductIds = new SalesDataService().GetAllProductsForActiveOrders();

            foreach (var reservedProductId in reservedProductIds.Distinct())
            {
                var productCode = new ProductDataService().GetProductById(reservedProductId).Code;
                var reservedAmount = new SalesDataService().GetReservedCountyProductId(reservedProductId);
                allStocks.First(x => x.Code == productCode).ReservedAmount = reservedAmount;
                allStocks.First(x => x.Code == productCode).RemainingAmount =
                    allStocks.First(x => x.Code == productCode).RemainingAmount - reservedAmount;
            }

            var orderedStocks = allStocks.OrderByDescending(x => x.ReservedAmount);

            return new JsonResult()
            {
                Data = orderedStocks,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public ActionResult GetStock(string productCode)
        {
            var orkaDataService = new ORKADataService();
            var stocks = orkaDataService.GetStockByCodeWithWildCard(productCode).Select(x => new StockViewModel { Code = x.Code, Price = x.Price, RemainingAmount = x.RemainingAmount }).ToList();

            var productIds = new ProductDataService().GetProductByCodeWithWildCard(productCode).Select(x => x.Id).ToList();

            foreach (var productId in productIds)
            {
                var code = new ProductDataService().GetProductById(productId).Code;
                var reservedAmount = new SalesDataService().GetReservedCountyProductId(productId);
                stocks.First(x => x.Code == code).ReservedAmount = reservedAmount;
                stocks.First(x => x.Code == code).RemainingAmount =
                    stocks.First(x => x.Code == code).RemainingAmount - reservedAmount;
            }


            return new JsonResult()
            {
                Data = stocks,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}