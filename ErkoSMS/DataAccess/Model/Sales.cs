using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNet.Identity.SQLite;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ErkoSMS.DataAccess.Model
{
    public class Sales
    {
        public DateTime SalesStartDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public SalesState SalesState { get; set; }
        public int Id { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public Customer Customer { get; set; }
        public string SalesUserGuid { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Currency Currency { get; set; }
        public double TotalPrice { get; set; }
        public IEnumerable<SalesDetail> SalesDetails { get; set; }

        public double ExchangeRate { get; set; }
        public Exporter Exporter { get; set; }
        public string Comment { get; set; }
        public double TransportCost { get; set; }
        public string DeliveryType { get; set; }
        public string PaymentType { get; set; }
    }
}
