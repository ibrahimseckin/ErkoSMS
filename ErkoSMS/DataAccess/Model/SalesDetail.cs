namespace ErkoSMS.DataAccess.Model
{
    public class SalesDetail
    {
        public int SalesId { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string ProductEnglishDescription { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
