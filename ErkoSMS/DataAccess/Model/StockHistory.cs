using System;

namespace ErkoSMS.DataAccess.Model
{
    public class StockHistory
    {
        public int StockId { get; set; }
        public StockChangeState Change { get; set; }
        public int ChangeAmount { get; set; }
        public DateTime ChangeTime { get; set; }
    }
}
