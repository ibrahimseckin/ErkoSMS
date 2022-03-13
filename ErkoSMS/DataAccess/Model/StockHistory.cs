using System;

namespace ErkoSMS.DataAccess.Model
{
    public class StockHistory
    {
        public string Code { get; set; }
        public int Amount { get; set; }
        public string Change { get; set; }
        public int ChangeAmount { get; set; }
        public DateTime ChangeTime { get; set; }
    }
}
