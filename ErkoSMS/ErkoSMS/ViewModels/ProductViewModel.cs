using ErkoSMS.DataAccess.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ErkoSMS.ViewModels
{
    public class ProductViewModel : IProduct
    {
        public int Id { get; set; }

        [Display(Name = "Ürün Kodu")]
        public string Code { get; set; }

        [Display(Name = "Referans Kodu")]
        public string CrossReferenceCode { get; set; }

        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Display(Name = "Açıklama (İngilizce)")]
        public string EnglishDescription { get; set; }

        [Display(Name = "Group")]
        public string Group { get; set; }

        [Display(Name = "Marka")]
        public string Brand { get; set; }

        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "Son Satış Fiyatı")]
        public string LastPrice { get; set; }

    }
}