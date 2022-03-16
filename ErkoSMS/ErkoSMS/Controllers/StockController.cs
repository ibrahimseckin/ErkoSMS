using ErkoSMS.DataAccess;
using ErkoSMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ErkoSMS.DataAccess.Interfaces;

namespace ErkoSMS.Controllers
{
    public class StockController : Controller
    {
        // GET: Stock
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult History()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAllStocks()
        {
            var stockHistoryHelper = new StockHistoryHelper();
            var allStocks = stockHistoryHelper.GetAllStocks();
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
            var orkaDataService = new StockDataService();
            var stocks = orkaDataService.GetStockByCodeWithWildCardFromOrka(productCode).Select(x => new StockViewModel { Code = x.Code, Price = x.Price, StockAmount = x.StockAmount }).ToList();

            var productIds = new ProductDataService().GetProductByCodeWithWildCard(productCode).Select(x => x.Id).ToList();

            foreach (var productId in productIds)
            {
                var code = new ProductDataService().GetProductById(productId).Code;
                var reservedAmount = new SalesDataService().GetReservedCountyProductId(productId);
                var stock = stocks.FirstOrDefault(x => x.Code == code);
                if (stock != null)
                {
                    stock.ReservedAmount = reservedAmount;
                    stock.StockAmount -= reservedAmount;
                }
            }


            return new JsonResult()
            {
                Data = stocks,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpGet]
        public ActionResult GetAllStockActivities()
        {
            var stockDataService = new StockDataService();
            var stockHistoryActivities = stockDataService.GetAllStockHistory();
            var stockHistoryViewModels = new List<StockHistoryViewModel>();
            foreach (var stockHistory in stockHistoryActivities)
            {
                stockHistoryViewModels.Add(new StockHistoryViewModel
                {
                    ProductCode = stockDataService.GetProductCodeByStockId(stockHistory.StockId),
                    Change = stockHistory.Change,
                    ChangeAmount = stockHistory.ChangeAmount,
                    ChangeTime = stockHistory.ChangeTime
                });
            }


            var stockHistoryOrderedByProductCode = stockHistoryViewModels.OrderBy(x => x.ProductCode).ToList();
            return new JsonResult()
            {
                Data = stockHistoryOrderedByProductCode,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}