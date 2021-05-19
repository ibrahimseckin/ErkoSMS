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
            this.InvoiceNumber = sales.InvoiceNumber;
            this.CustomerId = sales.Customer.Id;
            this.State = sales.SalesState;
            this.Currency = sales.Currency;
            this.OrderId = sales.Id;
            this.OrderLines = new List<OrderLine>();
            foreach (var salesDetail in sales.SalesDetails)
            {
                var productCode = salesDetail.ProductCode;
                var quantity = salesDetail.Quantity;
                var unitPrice = salesDetail.UnitPrice;
                var productDescription = salesDetail.ProductDescription;
                OrderLines.Add(new OrderLine
                {
                    TotalPrice = quantity * unitPrice,
                    ProductCode = productCode,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    ProductDescription = productDescription
                });
            }
        }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public Currency Currency { get; set; }
        public double TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public IList<OrderLine> OrderLines { get; set; }
        public SalesState State { get; set; }
        [Display(Name = "Fatura No:")]
        public string InvoiceNumber { get; set; }
        [Display(Name = "Fatura Tarihi:")]
        public DateTime InvoiceDate { get; set; }
    }

    public class OrderLine
    {
        [Display(Name = "Kod")]
        public string ProductCode { get; set; }
        [Display(Name = "Açıklama")]
        public string ProductDescription { get; set; }
        [Display(Name = "Adet")]
        public int Quantity { get; set; }
        [Display(Name = "Br.Fiyat")]
        public double UnitPrice { get; set; }
        [Display(Name = "Top.Fiyat")]
        public double TotalPrice { get; set; }
    }

}