using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErkoSMS.DataAccess.Model;

namespace ErkoSMS.DataAccess
{
    public class PurchaseDataService
    {
        private IDataProvider _sqliteDataProvider;
        public PurchaseDataService()
        {
            _sqliteDataProvider = new SqliteDataProvider();
        }

        public IEnumerable<Purchase> GetAllPurchases()
        {
            const string query = "select * from purchases";
            var dataset = _sqliteDataProvider.ExecuteDataSet(query);
            IList<Purchase> purchases = new List<Purchase>();
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                purchases.Add(CreatePurchase(row));
            }

            return purchases;
        }

        private Purchase CreatePurchase(DataRow row)
        {
            return new Purchase
            {
                PurchaseId = Convert.ToInt32(row["Id"]),
                ProductId = Convert.ToInt32(row["ProductId"]),
                ProductCode = row["ProductCode"].ToString(),
                PurchaseStartDate = DateTime.Parse(row["StartDate"].ToString()),
                PurchaserUserGuid = row["AssignedUser"].ToString(),
                SupplierId = Convert.ToInt32(row["SupplierId"]),
                PurchaseState = (PurchaseState)Convert.ToInt32(row["State"]),
                UnitPrice = Convert.ToInt32(row["UnitPrice"]),
                TotalPrice = Convert.ToInt32(row["TotalPrice"]),
                Quantity = Convert.ToInt32(row["Amount"])
            };
        }

        public bool CreatePurchase(Purchase purchase)
        {
            const string query = "Insert into Purchases (ProductId,ProductCode,Amount,State,StartDate,SaleId,AssignedUser,TotalPrice," +
                                 "UnitPrice,SupplierId,Currency,RequestedBySales,SalesUser) values " +
                                 "(@ProductId,@ProductCode,@Amount,@State,@StartDate,@SaleId,@AssignedUser,@TotalPrice,@UnitPrice," +
                                 "@SupplierId,@Currency,@RequestedBySales,@SalesUser);" +
                                 "select last_insert_rowid();";
            _sqliteDataProvider.AddParameter("@ProductId", purchase.ProductId);
            _sqliteDataProvider.AddParameter("@ProductCode", purchase.ProductCode);
            _sqliteDataProvider.AddParameter("@Amount", purchase.Quantity);
            _sqliteDataProvider.AddParameter("@State", purchase.PurchaseState);
            _sqliteDataProvider.AddParameter("@StartDate", purchase.PurchaseStartDate);
            _sqliteDataProvider.AddParameter("@SaleId", purchase.OrderId);
            _sqliteDataProvider.AddParameter("@SalesUser", purchase.SalesUserName);
            _sqliteDataProvider.AddParameter("@AssignedUser", string.IsNullOrEmpty(purchase.PurchaserUserGuid) ?
                null : purchase.PurchaserUserGuid);
            _sqliteDataProvider.AddParameter("@TotalPrice", purchase.TotalPrice);
            _sqliteDataProvider.AddParameter("@UnitPrice", purchase.UnitPrice);
            _sqliteDataProvider.AddParameter("@SupplierId", purchase.SupplierId);
            _sqliteDataProvider.AddParameter("@Currency", purchase.Currency);
            _sqliteDataProvider.AddParameter("@RequestedBySales", purchase.RequestedBySales);
            try
            {
                var queryResult = _sqliteDataProvider.ExecuteScalar(query);
            }
            catch (Exception e)
            {
                var a = 5;
            }

            return false;
        }

    }
}
