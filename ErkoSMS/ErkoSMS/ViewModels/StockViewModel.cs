using ErkoSMS.DataAccess.Interfaces;

namespace ErkoSMS.ViewModels
{
    public class StockViewModel : IStock
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int StockAmount { get; set; }
        public int ReservedAmount { get; set; }
        public double Price { get; set; }


    }
}