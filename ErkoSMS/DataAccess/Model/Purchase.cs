using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess.Model
{
    public class Purchase
    {
        public int PurchaseId { get; set; }
        public DateTime PurchaseStartDate { get; set; }
        public PurchaseState PurchaseState { get; set; }
        public int SupplierId { get; set; }
        public string PurchaserUserGuid { get; set; }
        public string SalesUserName { get; set; }
        public Currency Currency { get; set; }
        public double TotalPrice { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public bool RequestedBySales { get; set; }
        public int? OrderId { get; set; }
    }
}
