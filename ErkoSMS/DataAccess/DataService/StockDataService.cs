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
        private IDataProvider _orkaDataProvider;
        public StockDataService()
        {
            _sqliteDataProvider = new SqliteDataProvider();
            _orkaDataProvider = new MsSqlDataProvider();
        }

        public IList<IStock> GetAllStocksFromOrka()
        {
            const string query = "SELECT sm.stokkodu, sm.kalanmiktar, ss.fiyat FROM dbo.[STOK_MIZAN] sm " +
                                 "join dbo.STK_STOKSATIR ss on sm.stokkodu = ss.stokkodu " +
                                 "WHERE ss.firstdate IN (SELECT max(ss2.firstdate) FROM dbo.STK_STOKSATIR ss2 WHERE ss2.stokkodu=ss.stokkodu and ss2.IOdurum = 1) and ss.IOdurum = 1";
            var dataset = _orkaDataProvider.ExecuteDataRows(query);
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
                                 "WHERE ss.firstdate IN (SELECT max(ss2.firstdate) FROM dbo.STK_STOKSATIR ss2 WHERE ss2.stokkodu=ss.stokkodu and ss2.IOdurum = 1) AND lower(sm.stokkodu) = lower(@stokkodu) and ss.IOdurum = 1"; _orkaDataProvider.AddParameter("@stokkodu", stockCode);
            var row = _orkaDataProvider.ExecuteDataRows(query).FirstOrDefault();
            return row != null ? CreateStockObject(row) : null;
        }

        public IList<IStock> GetStockByCodeWithWildCardFromOrka(string stockCode)
        {
            const string query = "SELECT sm.stokkodu, sm.kalanmiktar, ss.fiyat FROM dbo.[STOK_MIZAN] sm " +
                                 "join dbo.STK_STOKSATIR ss on sm.stokkodu = ss.stokkodu " +
                                 "WHERE ss.firstdate IN (SELECT max(ss2.firstdate) FROM dbo.STK_STOKSATIR ss2 WHERE ss2.stokkodu=ss.stokkodu and ss2.IOdurum = 1) AND lower(sm.stokkodu) like lower(@stokkodu) and ss.IOdurum = 1";
            _orkaDataProvider.AddParameter("@stokkodu", $"%{stockCode}%");
            var rows = _orkaDataProvider.ExecuteDataRows(query);
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
                "INSERT INTO Stock (ProductCode,Amount,ReservedAmount) VALUES(@productCode,@stockAmount,@reservedAmount)";
            foreach (var stock in stocks)
            {
                _sqliteDataProvider.AddParameter("@productCode",stock.Code);
                _sqliteDataProvider.AddParameter("@stockAmount", stock.StockAmount);
                _sqliteDataProvider.AddParameter("@reservedAmount", stock.ReservedAmount);
                _sqliteDataProvider.ExecuteScalar(query);
            }
        }

        public void UpdateStock(IList<IStock> stocks)
        {
            string query =
                "Update Stock Set Amount = @stockAmount where Id = @Id";
            foreach (var stock in stocks)
            {
                _sqliteDataProvider.AddParameter("@stockAmount", stock.StockAmount);
                _sqliteDataProvider.AddParameter("@Id", stock.Id);
                _sqliteDataProvider.ExecuteScalar(query);
            }
        }

        public Dictionary<string,int> GetAllProductsInStock()
        {
            const string query = "Select productCode,Id from Stock";
            var dataset = _sqliteDataProvider.ExecuteDataSet(query);

            var productDictionary = new Dictionary<string, int>();
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                productDictionary[row["ProductCode"].ToString()] = Convert.ToInt32(row["Id"].ToString());
            }

            return productDictionary;
        }

        public void InsertStockHistory(IList<StockHistory> stockHistories)
        {
            string query =
                "INSERT INTO StockHistory (StockId,Change,ChangeDate,ChangeAmount) " +
                "VALUES(@stockId,@Change,@ChangeDate,@ChangeAmount)";
            foreach (var stockHistory in stockHistories)
            {
                _sqliteDataProvider.AddParameter("@stockId", stockHistory.StockId);
                _sqliteDataProvider.AddParameter("@Change", stockHistory.Change);
                _sqliteDataProvider.AddParameter("@changedate", stockHistory.ChangeTime);
                _sqliteDataProvider.AddParameter("@changeamount", stockHistory.ChangeAmount);
                _sqliteDataProvider.ExecuteScalar(query);
            }
        }

        public IList<StockHistory> GetAllStockHistory()
        {
            var stockHistories = new List<StockHistory>();

            const string query = "Select stockId,change,changedate,changeamount from StockHistory";
            var dataset = _sqliteDataProvider.ExecuteDataSet(query);

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                stockHistories.Add(new StockHistory
                {
                    Change = (StockChangeState)Convert.ToInt32(row["change"]),
                    ChangeAmount = Convert.ToInt32(row["changeamount"].ToString()),
                    ChangeTime = DateTime.Parse(row["changedate"].ToString()),
                    StockId = Convert.ToInt32(row["stockId"].ToString())
            });
            }

            return stockHistories;
        }

        public string GetProductCodeByStockId(int stockId)
        {
            const string query = "select productCode from Stock where id = @stockId";
            _sqliteDataProvider.AddParameter("@stockId", stockId);

            var dataSet = _sqliteDataProvider.ExecuteDataSet(query);
            return dataSet.Tables[0].Rows[0]["productCode"].ToString();
        }

        public IList<IStock> GetAllStocks()
        {
            const string query = "select productCode,amount,reservedamount from Stock";
            var dataset = _sqliteDataProvider.ExecuteDataSet(query);
            var stocks = new List<IStock>();

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                stocks.Add(new Stock
                {
                    Code = row["productCode"].ToString(),
                    StockAmount = Convert.ToInt32(row["amount"].ToString()),
                    ReservedAmount = Convert.ToInt32(row["reservedamount"].ToString())
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
