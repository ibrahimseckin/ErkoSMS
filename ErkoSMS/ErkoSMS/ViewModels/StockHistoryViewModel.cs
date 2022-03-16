using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using ErkoSMS.DataAccess.Model;

namespace ErkoSMS.ViewModels
{
    public class StockHistoryViewModel
    {
        public int StockId { get; set; }
        public StockChangeState Change { get; set; }
        public int ChangeAmount { get; set; }
        public DateTime ChangeTime { get; set; }
        public string ProductCode { get; set; }
    }
}