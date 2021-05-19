using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess.Model
{
    public enum PurchaseState
    {
        [Display(Name = "Satın Alma Talep Edildi")]
        PurchaseRequested = 0,
        [Display(Name = "Satın Alma İşleme Alındı")]
        PurchaseInProgress = 1,
        [Display(Name = "Satın Alma Başarılı")]
        PurchaseSuccesful = 2,
        [Display(Name = "Satın Alma Başarısız")]
        PurchaseFailed = 3
    }
}
