using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.Interfaces;
using ErkoSMS.DataAccess.Model;
using ErkoSMS.ViewModels;

namespace ErkoSMS
{
    public class StockHistoryHelper
    {
        public IList<StockViewModel> GetAllStocks()
        {
            var orkaDataService = new StockDataService();
            var allStocks = orkaDataService.GetAllStocksFromOrka().Select(x => new StockViewModel { Code = x.Code, Price = x.Price, StockAmount = x.StockAmount }).ToList();
            var reservedProductIds = new SalesDataService().GetAllProductsForActiveOrders();

            foreach (var reservedProductId in reservedProductIds.Distinct())
            {
                var productCode = new ProductDataService().GetProductById(reservedProductId).Code;
                var reservedAmount = new SalesDataService().GetReservedCountyProductId(reservedProductId);
                var stock = allStocks.FirstOrDefault(x => x.Code == productCode);
                if (stock != null)
                {
                    stock.ReservedAmount = reservedAmount;
                    stock.StockAmount -= reservedAmount;
                }

            }

            return allStocks;
        }

        public void UpdateStockHistory()
        {
            var stockDataService = new StockDataService();
            var existingProductsInStock = stockDataService.GetAllProductsInStock();

            var allStockInformation = GetAllStocks();
            var stocksWillBeAdded = new List<IStock>();
            var stockHistoryWillBeAdded = new List<StockHistory>();
            foreach (var stockInformation in allStockInformation)
            {
                //stock does not exist, it should be added to stock
                if (existingProductsInStock.ContainsKey(stockInformation.Code) == false)
                {
                    stocksWillBeAdded.Add(new Stock
                    {
                        Code = stockInformation.Code,
                        StockAmount = stockInformation.StockAmount,
                        ReservedAmount = stockInformation.ReservedAmount
                    });
                }
                else
                {
                    //Get previous stock information
                    var previousStocks = stockDataService.GetAllStocks();

                    //Compare previous and existing stock amounts
                    
                    foreach (var previousStock in previousStocks)
                    {
                        var time = DateTime.Now;
                        var existingStock = allStockInformation.First(x => x.Code == previousStock.Code);
                        if (existingStock != null && previousStock.StockAmount !=
                            existingStock.StockAmount)
                        {
                            //insert to history list; 
                            var stockHistory = new StockHistory();
                            stockHistory.StockId = existingProductsInStock[previousStock.Code.ToString()];
                            stockHistory.ChangeAmount = Math.Abs(
                                existingStock.StockAmount - previousStock.StockAmount);
                            stockHistory.Change = existingStock.StockAmount > previousStock.StockAmount
                                ? StockChangeState.StokIncreased
                                : StockChangeState.StokDecreased;
                            stockHistory.ChangeTime = time;
                        }
                    }
                }
            }

            stockDataService.InsertStockHistory(stockHistoryWillBeAdded);
            stockDataService.InsertStock(stocksWillBeAdded);
        }


    }
}