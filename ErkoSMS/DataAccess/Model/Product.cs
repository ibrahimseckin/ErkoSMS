using ErkoSMS.DataAccess.Interfaces;

namespace ErkoSMS.DataAccess.Model
{
    public class Product : IProduct
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string CrossReferenceCode { get; set; }
        public string Description { get; set; }
        public string EnglishDescription { get; set; }
        public string Group { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public double LastPrice { get; set; }

    }
}
