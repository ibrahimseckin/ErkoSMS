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

            var stockDataService = new StockDataService();
            var reservedStocks = stockDataService.GetReservedStocks();

            foreach (var reservedStock in reservedStocks)
            {
                var stock = allStocks.FirstOrDefault(x => x.Code == reservedStock.Product.Code);
                if (stock != null)
                {
                    stock.ReservedAmount = reservedStock.Reserved;
                }
            }

            return new JsonResult()
            {
                Data = allStocks,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public ActionResult GetStock(string productCode)
        {
            var orkaDataService = new ORKADataService();
            var stock = orkaDataService.GetStockByCode(productCode);
            var stockViewModel = new StockViewModel();
            if (stock != null)
            {
                stockViewModel.Code = stock.Code;
                stockViewModel.Price = stock.Price;
                stockViewModel.RemainingAmount = stock.RemainingAmount;

                var stockDataService = new StockDataService();
                var reservedStock = stockDataService.GetReservedStockByCode(productCode);
                if (reservedStock != null)
                {
                    stockViewModel.ReservedAmount = reservedStock.Reserved;
                }

            }

            return new JsonResult()
            {
                Data = stockViewModel != null ? new List<StockViewModel> { stockViewModel } : new List<StockViewModel>(),
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}