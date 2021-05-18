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
        }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public Currency Currency { get; set; }
        public double TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public IEnumerable<OrderLine> OrderLines { get; set; }
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
        [Display(Name = "Adet")]
        public int Quantity { get; set; }
        [Display(Name = "Br.Fiyat")]
        public double UnitPrice { get; set; }
        [Display(Name = "Top.Fiyat")]
        public double TotalPrice { get; set; }
    }

}