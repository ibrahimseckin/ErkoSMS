using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ErkoSMS.ViewModels
{
    public class OrderViewModel
    {
        public DateTime OrderDate { get; set; }
        public IEnumerable<OrderLine> OrderLines { get; set; }
    }

    public class OrderLine
    {
        [Display(Name = "Ürün Kodu")]
        public string ProductCode { get; set; }
        [Display(Name = "Adet")]
        public int Quantity { get; set; }
        [Display(Name = "Birim Fiyatı")]
        public double UnitPrice { get; set; }
    }
}