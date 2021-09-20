using ErkoSMS.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ErkoSMS.Models;

namespace ErkoSMS.ViewModels
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
        }
        public OrderViewModel(Sales sales)
        {
            this.TotalPrice = sales.TotalPrice;
            this.TotalPriceTL = sales.TotalPrice * (sales.Currency != Currency.Tl ? sales.ExchangeRate : 1.0);
            this.InvoiceNumber = sales.InvoiceNumber;
            if(sales.InvoiceDate.HasValue)
                this.InvoiceDate = sales.InvoiceDate.Value;
            this.SalesStartDate = sales.SalesStartDate;
            this.Customer = sales.Customer;
            this.State = sales.SalesState;
            this.Currency = sales.Currency;
            this.ExchangeRate = sales.ExchangeRate;
            this.OrderId = sales.Id;
            this.SalesState = sales.SalesState;
            this.OrderLines = new List<OrderLine>();
            if (sales.SalesDetails != null)
            {
                foreach (var salesDetail in sales.SalesDetails)
                {
                    var productCode = salesDetail.ProductCode;
                    var quantity = salesDetail.Quantity;
                    var unitPrice = salesDetail.UnitPrice;
                    var productDescription = salesDetail.ProductDescription;
                    OrderLines.Add(new OrderLine
                    {
                        TotalPriceTL = quantity * unitPrice * (Currency != Currency.Tl ? ExchangeRate : 1.0),
                        TotalPrice = quantity * unitPrice,
                        ProductCode = productCode,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        ProductDescription = productDescription
                    });
                }
            }
        }
        public int OrderId { get; set; }
        public Customer Customer { get; set; }
        public Currency Currency { get; set; }
        public double ExchangeRate { get; set; }
        public double TotalPrice { get; set; }
        public double TotalPriceTL { get; set; }
        public DateTime OrderDate { get; set; }
        public IList<OrderLine> OrderLines { get; set; }
        public SalesState State { get; set; }
        [Display(Name = "Fatura No:")]
        public string InvoiceNumber { get; set; }
        [Display(Name = "Fatura Tarihi:")]
        public DateTime? InvoiceDate { get; set; }
        public DateTime SalesStartDate { get; set; }
        public SalesState SalesState { get; set; }
    }

    public class OrderLine
    {
        [Display(Name = "Kod")]
        public string ProductCode { get; set; }
        [Display(Name = "Açıklama")]
        public string ProductDescription { get; set; }
        [Display(Name = "Adet")]
        public int Quantity { get; set; }
        [Display(Name = "Stok Adedi")]
        public int StokQuantity { get; set; }
        [Display(Name = "Br.Fiyat")]
        public double UnitPrice { get; set; }
        [Display(Name = "Top.Fiyat")]
        public double TotalPrice { get; set; }
        [Display(Name = "Top.Fiyat (TL)")]
        public double TotalPriceTL { get; set; }
    }

}