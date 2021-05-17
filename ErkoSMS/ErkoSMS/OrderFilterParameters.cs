using ErkoSMS.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ErkoSMS
{
    public class OrderFilterParameters
    {
        public SalesState? State { get; set; }
        public IEnumerable<string> Customers { get; set; }
        public Currency? Currency { get; set; }
        public string InvoiceNumber { get; set; }

    }
}