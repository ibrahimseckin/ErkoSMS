using ErkoSMS.DataAccess.Interfaces;
using ErkoSMS.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data;
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

        public IList<IStock> GetReservedStocks()
        {
            const string query = "select s.ID, p.Code, s.Reserved from Stock s join Products p on s.ProductId = p.Id";
            var dataset = _sqliteDataProvider.ExecuteDataSet(query);
            IList<IStock> stocks = new List<IStock>();
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                stocks.Add(CreateStockObject(row));
            }

            return stocks;
        }


        public IStock GetReservedStockByCode(string productCode)
        {
            const string query = "select s.ID, p.Code, s.Reserved from Stock s join Products p on s.ProductId = p.Id where p.Code = @productCode";
            _sqliteDataProvider.AddParameter("@productCode", productCode);

            DataRow row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            return row != null ? CreateStockObject(row) : null;
        }


        public IList<IStock> GetReservedStockByCodeWithWildCard(string productCode)
        {
            const string query = "select s.ID, p.Code, s.Reserved from Stock s join Products p on s.ProductId = p.Id where p.Code like  @productCode";
            _sqliteDataProvider.AddParameter("@productCode", $"%{productCode}%");

            var rows = _sqliteDataProvider.ExecuteDataRows(query);
            IList<IStock> stocks = new List<IStock>();
            foreach (DataRow row in rows)
            {
                stocks.Add(CreateStockObject(row));
            }

            return stocks;
        }


        private IStock CreateStockObject(DataRow row)
        {
            var stock = new Stock
            {
                Id = Convert.ToInt32(row["Id"]),
                Product = new Product
                {
                    Code = row["Code"]?.ToString()
                },
                Reserved = Convert.ToInt32(row["Reserved"])
            };

            return stock;
        }
    }
}
