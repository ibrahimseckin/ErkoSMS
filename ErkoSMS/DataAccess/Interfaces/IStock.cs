namespace ErkoSMS.DataAccess.Interfaces
{
    public interface IStock
    {
         int Id { get; set; }
         string Code { get; set; }
         int StockAmount { get; set; }
         int ReservedAmount { get; set; }
         double Price { get; set; }
    }
}
