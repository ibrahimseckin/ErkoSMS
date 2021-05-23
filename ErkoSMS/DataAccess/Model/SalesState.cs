using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ErkoSMS.DataAccess.Model
{
    public enum SalesState
    {
        //Nonspecified is for old data
        [Display(Name = "Belirtilmemiş")]
        Nonspecified = -1,
        [Display(Name = "İç Satış İletildi")]
        InternalSalesConducted = 0,
        [Display(Name = "Fatura Kesildi ve Paketlendi")]
        InvoiceDoneAndPacked = 1,
        [Display(Name = "Paketleme Bekleniyor")]
        WaitForPacking = 2,
        [Display(Name = "Transfer Bekliyor")]
        WaitForTransfer = 3,
        [Display(Name = "Ödeme Yapıldı - Transfer Bekleniyor")]
        PaymentDoneAndWaitForTransfer = 4,
        [Display(Name = "Paketleme Hazır")]
        PackingIsReady = 5,
        [Display(Name = "Paketleme Hazır - Ödeme Bekleniyor")]
        PackingIsReadyAndWaitForPayment = 6,
        [Display(Name = "Reddedildi")]
        Rejected = 7,
        [Display(Name = "Cevap Bekliyor")]
        WaitForAnswer = 8,
        [Display(Name = "Satın Alma Talep Edildi")]
        PurchaseRequested = 9,
        [Display(Name = "Satın Alma Talebi İşleme Alındı")]
        PurchaseInProgress = 10,
        [Display(Name = "Satın Alma Başarılı Tamamlandı")]
        PurchaseSuccesful = 11,
        [Display(Name = "Satın Alma Talebi Başarısız Sonuçlandı")]
        PurchaseFailed = 12
    }
}
