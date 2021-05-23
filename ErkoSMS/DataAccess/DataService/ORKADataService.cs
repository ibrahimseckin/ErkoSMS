using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErkoSMS.DataAccess.Interfaces;
using ErkoSMS.DataAccess.Model;

namespace ErkoSMS.DataAccess
{
    public class ORKADataService
    {
        private IDataProvider _sqliteDataProvider;
        public ORKADataService()
        {
            _sqliteDataProvider = new MsSqlDataProvider();
        }

        public IList<IStockORKA> GetAllStocks()
        {
            const string query = "SELECT sm.stokkodu, sm.kalanmiktar, ss.fiyat FROM dbo.[STOK_MIZAN] sm " +
                "join dbo.STK_STOKSATIR ss on sm.stokkodu = ss.stokkodu " +
                "WHERE ss.firstdate IN (SELECT max(ss2.firstdate) FROM dbo.STK_STOKSATIR ss2 WHERE ss2.stokkodu=ss.stokkodu)";
            var dataset = _sqliteDataProvider.ExecuteDataRows(query);
            IList<IStockORKA> stocks = new List<IStockORKA>();

            foreach (var row in dataset)
            {
                stocks.Add(CreateStockObject(row));
            }
            return stocks;
        }

        public IStockORKA GetStockByCode(string stockCode)
        {
            const string query = "SELECT sm.stokkodu, sm.kalanmiktar, ss.fiyat FROM dbo.[STOK_MIZAN] sm " +
                                "join dbo.STK_STOKSATIR ss on sm.stokkodu = ss.stokkodu " +
                                "WHERE ss.firstdate IN (SELECT max(ss2.firstdate) FROM dbo.STK_STOKSATIR ss2 WHERE ss2.stokkodu=ss.stokkodu) AND sm.stokkodu = @stokkodu";
            _sqliteDataProvider.AddParameter("@stokkodu", stockCode);
            var row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            return row != null ? CreateStockObject(row) : null;
        }


        private IStockORKA CreateStockObject(DataRow row)
        {
            return new StockORKA
            {
                Code = row["stokkodu"].ToString(),
                RemainingAmount = Convert.ToInt32(row["kalanmiktar"].ToString()),
                Price = Convert.ToDouble(row["fiyat"].ToString())
            };
        }
    }
}
