using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNet.Identity.SQLite;

namespace ErkoSMS.DataAccess.Model
{
    public class Sales
    {
        public DateTime SalesStartDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public SalesState SalesState { get; set; }
        public int Id { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int InvoiceNumber { get; set; }
        public Customer Customer { get; set; }
        public IdentityUser SalesUser { get; set; }
        public Currency Currency { get; set; }
        public double TotalPrice { get; set; }
    }
}
