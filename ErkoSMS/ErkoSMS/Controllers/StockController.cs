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
                var stock = allStocks.FirstOrDefault(x => x.Code == productCode);
                if (stock != null)
                {
                    stock.ReservedAmount = reservedAmount;
                    stock.RemainingAmount -= reservedAmount;
                }

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
                var stock = stocks.FirstOrDefault(x => x.Code == code);
                if (stock != null)
                {
                    stock.ReservedAmount = reservedAmount;
                    stock.RemainingAmount -= reservedAmount;
                }
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