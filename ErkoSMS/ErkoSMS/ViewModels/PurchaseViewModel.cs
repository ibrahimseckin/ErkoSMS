using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ErkoSMS.DataAccess.Interfaces;
using ErkoSMS.DataAccess.Model;

namespace ErkoSMS.ViewModels
{
    public class PurchaseViewModel
    {
        public int PurchaseId { get; set; }

        [Display(Name = "Tedarikçi")]
        public int SupplierId { get; set; }
        public string PurchaserUser { get; set; }
        [Display(Name = "Satış Elemanı")]
        public string SalesUserName { get; set; }
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime PurchaseStartDate { get; set; }
        [Display(Name = "Durum")]
        public PurchaseState PurchaseState { get; set; }
        [Display(Name = "Kur")]
        public Currency Currency { get; set; }
        [Display(Name = "Toplam Fiyat")]
        public double TotalPrice { get; set; }
        [Display(Name = "Adet")]
        public int Quantity { get; set; }
        [Display(Name = "Birim Fiyatı")]
        public double UnitPrice { get; set; }
        public int ProductId { get; set; }
        [Display(Name = "Ürün Kodu")]
        public string ProductCode { get; set; }
        public bool RequestedBySales { get; set; }
        public int OrderId { get; set; }

        public PurchaseViewModel()
        {
        }

        public PurchaseViewModel(Purchase purchase)
        {
            SupplierId = purchase.SupplierId;
            PurchaseId = purchase.PurchaseId;
            OrderId = purchase.OrderId.HasValue ? purchase.OrderId.Value : 0;
            ProductCode = purchase.ProductCode;
            PurchaseState = purchase.PurchaseState;
            Quantity = purchase.Quantity;
            Currency = purchase.Currency;
            UnitPrice = purchase.UnitPrice;
            TotalPrice = purchase.TotalPrice;
            PurchaseStartDate = purchase.PurchaseStartDate;
            RequestedBySales = purchase.RequestedBySales;
            SalesUserName = purchase.SalesUserName;
        }
    }
}