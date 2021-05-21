using ErkoSMS.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ErkoSMS
{
    public class PurchaseFilterParameters
    {
        [Display(Name="Durum")]
        public PurchaseState? State { get; set; }
        [Display(Name = "Tedarikçi")]
        public int SupplierId { get; set; }
    }
}