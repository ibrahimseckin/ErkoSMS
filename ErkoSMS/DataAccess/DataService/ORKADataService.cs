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
                "WHERE ss.firstdate IN (SELECT max(ss2.firstdate) FROM dbo.STK_STOKSATIR ss2 WHERE ss2.stokkodu=ss.stokkodu) and ss.IOdurum = 1";
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
                                "WHERE ss.firstdate IN (SELECT max(ss2.firstdate) FROM dbo.STK_STOKSATIR ss2 WHERE ss2.stokkodu=ss.stokkodu) AND lower(sm.stokkodu) = lower(@stokkodu) and ss.IOdurum = 1";
            _sqliteDataProvider.AddParameter("@stokkodu", stockCode);
            var row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            return row != null ? CreateStockObject(row) : null;
        }


        public IList<IStockORKA> GetStockByCodeWithWildCard(string stockCode)
        {
            const string query = "SELECT sm.stokkodu, sm.kalanmiktar, ss.fiyat FROM dbo.[STOK_MIZAN] sm " +
                                "join dbo.STK_STOKSATIR ss on sm.stokkodu = ss.stokkodu " +
                                "WHERE ss.firstdate IN (SELECT max(ss2.firstdate) FROM dbo.STK_STOKSATIR ss2 WHERE ss2.stokkodu=ss.stokkodu) AND lower(sm.stokkodu) like lower(@stokkodu) and ss.IOdurum = 1";
            _sqliteDataProvider.AddParameter("@stokkodu", $"%{stockCode}%");
            var rows = _sqliteDataProvider.ExecuteDataRows(query);
            IList<IStockORKA> stocks = new List<IStockORKA>();

            foreach (var row in rows)
            {
                stocks.Add(CreateStockObject(row));
            }
            return stocks;
        }



        private IStockORKA CreateStockObject(DataRow row)
        {
            return new StockORKA
            {
                Code = row["stokkodu"].ToString(),
                RemainingAmount = row.IsNull("kalanmiktar") ? -1 : Convert.ToInt32(row["kalanmiktar"].ToString()),
                Price = row.IsNull("fiyat") ? -1 : Convert.ToDouble(row["fiyat"].ToString())
            };
        }
    }
}
