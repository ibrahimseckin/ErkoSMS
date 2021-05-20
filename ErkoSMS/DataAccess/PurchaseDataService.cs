using System;
using System.Collections.Generic;
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
            var allPurchases = new List<Purchase>();
            return allPurchases;
        }

        public bool CreatePurchase(Purchase purchase)
        {
            const string query = "Insert into Purchases (ProductId,ProductCode,Amount,State,StartDate,SaleId,AssignedUser,TotalPrice," +
                                 "UnitPrice,SupplierId,Currency,RequestedBySales) values " +
                                 "(@ProductId,@ProductCode,@Amount,@State,@StartDate,@SaleId,@AssignedUser,@TotalPrice,@UnitPrice," +
                                 "@SupplierId,@Currency,@RequestedBySales);" +
                                 "select last_insert_rowid();";
            _sqliteDataProvider.AddParameter("@ProductId", purchase.ProductId);
            _sqliteDataProvider.AddParameter("@ProductCode", purchase.ProductCode);
            _sqliteDataProvider.AddParameter("@Amount", purchase.Quantity);
            _sqliteDataProvider.AddParameter("@State", purchase.PurchaseState);
            _sqliteDataProvider.AddParameter("@StartDate", purchase.PurchaseStartDate);
            _sqliteDataProvider.AddParameter("@SaleId", purchase.OrderId);
            _sqliteDataProvider.AddParameter("@AssignedUser", string.IsNullOrEmpty(purchase.PurchaserUserGuid) ? 
                DBNull.Value.ToString() : purchase.PurchaserUserGuid);
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
