using ErkoSMS.DataAccess.Interfaces;
using ErkoSMS.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess
{
    public class StockDataService
    {
        private IDataProvider _sqliteDataProvider;
        public StockDataService()
        {
            _sqliteDataProvider = new SqliteDataProvider();
        }

        public IList<IStock> GetAllStocksFromOrka()
        {
            const string query = "SELECT sm.stokkodu, sm.kalanmiktar, ss.fiyat FROM dbo.[STOK_MIZAN] sm " +
                "join dbo.STK_STOKSATIR ss on sm.stokkodu = ss.stokkodu " +
                "WHERE ss.firstdate IN (SELECT max(ss2.firstdate) FROM dbo.STK_STOKSATIR ss2 WHERE ss2.stokkodu=ss.stokkodu) and ss.IOdurum = 1";
            var dataset = _sqliteDataProvider.ExecuteDataRows(query);
            IList<IStock> stocks = new List<IStock>();

            foreach (var row in dataset)
            {
                stocks.Add(CreateStockObject(row));
            }
            return stocks;
        }

        public IStock GetStockByCodeFromOrka(string stockCode)
        {
            const string query = "SELECT sm.stokkodu, sm.kalanmiktar, ss.fiyat FROM dbo.[STOK_MIZAN] sm " +
                                "join dbo.STK_STOKSATIR ss on sm.stokkodu = ss.stokkodu " +
                                "WHERE ss.firstdate IN (SELECT max(ss2.firstdate) FROM dbo.STK_STOKSATIR ss2 WHERE ss2.stokkodu=ss.stokkodu) AND lower(sm.stokkodu) = lower(@stokkodu) and ss.IOdurum = 1";
            _sqliteDataProvider.AddParameter("@stokkodu", stockCode);
            var row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            return row != null ? CreateStockObject(row) : null;
        }

        public IList<IStock> GetStockByCodeWithWildCardFromOrka(string stockCode)
        {
            const string query = "SELECT sm.stokkodu, sm.kalanmiktar, ss.fiyat FROM dbo.[STOK_MIZAN] sm " +
                                "join dbo.STK_STOKSATIR ss on sm.stokkodu = ss.stokkodu " +
                                "WHERE ss.firstdate IN (SELECT max(ss2.firstdate) FROM dbo.STK_STOKSATIR ss2 WHERE ss2.stokkodu=ss.stokkodu) AND lower(sm.stokkodu) like lower(@stokkodu) and ss.IOdurum = 1";
            _sqliteDataProvider.AddParameter("@stokkodu", $"%{stockCode}%");
            var rows = _sqliteDataProvider.ExecuteDataRows(query);
            IList<IStock> stocks = new List<IStock>();

            foreach (var row in rows)
            {
                stocks.Add(CreateStockObject(row));
            }
            return stocks;
        }

        public void InsertStock(IList<IStock> stocks)
        {
            string query =
                "INSERT INTO Stock (ProductCode,StockAmount,ReservedAmount) VALUES(@productCode,@stockAmount,@reservedAmount)";
            foreach (var stock in stocks)
            {
                _sqliteDataProvider.AddParameter("@productCode",stock.Code);
                _sqliteDataProvider.AddParameter("@stockAmount", stock.StockAmount);
                _sqliteDataProvider.AddParameter("@reservedAmount", stock.ReservedAmount);
                _sqliteDataProvider.ExecuteScalar(query);
            }
        }

        public IList<string> GetAllProductCodesInStock()
        {
            const string query = "select productCode from Stock";
            var dataset = _sqliteDataProvider.ExecuteDataSet(query);

            return (from DataRow row in dataset.Tables[0].Rows 
                    select row["productCode"].ToString()).ToList();
        }

        public IList<IStock> GetAllStocks()
        {
            const string query = "select productCode,amount,reserved from Stock";
            var dataset = _sqliteDataProvider.ExecuteDataSet(query);
            var stocks = new List<IStock>();

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                stocks.Add(new Stock
                {
                    Code = row["productCode"].ToString(),
                    StockAmount = Convert.ToInt32(row["amount"].ToString()),
                    ReservedAmount = Convert.ToInt32(row["reserved"].ToString())
                });
            }

            return stocks;
        }

        private IStock CreateStockObject(DataRow row)
        {
            return new Stock()
            {
                Code = row["stokkodu"].ToString(),
                StockAmount = row.IsNull("kalanmiktar") ? -1 : Convert.ToInt32(row["kalanmiktar"].ToString()),
                Price = row.IsNull("fiyat") ? -1 : Convert.ToDouble(row["fiyat"].ToString())
            };
        }
    }
}
