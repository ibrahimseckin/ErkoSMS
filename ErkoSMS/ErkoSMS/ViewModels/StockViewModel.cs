using ErkoSMS.DataAccess.Interfaces;

namespace ErkoSMS.ViewModels
{
    public class StockViewModel : IStockORKA
    {
        public string Code { get; set; }
        public int RemainingAmount { get; set; }
        public int ReservedAmount { get; set; }
        public double Price { get; set; }


    }
}