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
            var previousStocks = stockDataService.GetAllStocks();
            var stocksWillBeAdded = new List<IStock>();
            var stockHistoryWillBeAdded = new List<StockHistory>();
            var stockWillBeUpdated = new List<IStock>();
            foreach (var existingStockInformation in allStockInformation)
            {
                //stock does not exist, it should be added to stock
                if (existingProductsInStock.ContainsKey(existingStockInformation.Code) == false)
                {
                    stocksWillBeAdded.Add(new Stock
                    {
                        Code = existingStockInformation.Code,
                        StockAmount = existingStockInformation.StockAmount,
                        ReservedAmount = existingStockInformation.ReservedAmount
                    });
                }
                else
                {
                    //Compare previous and existing stock amounts
                    var time = DateTime.Now;
                    //if there is a change on stock amount
                    var previousStock = previousStocks.First(x => x.Code == existingStockInformation.Code);
                    if (previousStock != null && previousStock.StockAmount !=
                        existingStockInformation.StockAmount)
                    {
                        //insert to updated stock list
                        stockWillBeUpdated.Add(new Stock
                        {
                            Id = existingProductsInStock[previousStock.Code.ToString()],
                            StockAmount = previousStock.StockAmount
                        });
                        //insert to history list; 
                        var stockHistory = new StockHistory
                        {
                            StockId = existingProductsInStock[previousStock.Code.ToString()],
                            ChangeAmount = Math.Abs(
                                existingStockInformation.StockAmount - previousStock.StockAmount),
                            Change = existingStockInformation.StockAmount > previousStock.StockAmount
                                ? StockChangeState.StokIncreased
                                : StockChangeState.StokDecreased,
                            ChangeTime = time
                        };
                        stockHistoryWillBeAdded.Add(stockHistory);
                    }
                }
            }

            stockDataService.InsertStockHistory(stockHistoryWillBeAdded);
            stockDataService.InsertStock(stocksWillBeAdded);
            stockDataService.UpdateStock(stockWillBeUpdated);
        }


    }
}