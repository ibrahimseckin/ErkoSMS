using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ErkoSMS.ViewModels
{
    public class ProductHitListViewModel
    {
        public string ProductName { get; set; }
        public int TotalCompletedOrderNumber { get; set; }
        public double TotalCompletedOrderIncomeTL { get; set; }
        public double TotalCompletedOrderIncomeEuro { get; set; }
        public double TotalCompletedOrderIncomeDollar { get; set; }

        public int TotalOngoingOrderNumber { get; set; }
        public double TotalOngoingOrderIncomeTL { get; set; }
        public double TotalOngoingOrderIncomeEuro { get; set; }
        public double TotalOngoingOrderIncomeDollar { get; set; }

    }
}