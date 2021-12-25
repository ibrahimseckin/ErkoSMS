using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErkoSMS.DataAccess.Interfaces;

namespace ErkoSMS.DataAccess.Model
{
    public class Exporter : IExporter
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string TradeRegisterNo { get; set; } = string.Empty;
        public string VatNo { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FaxNumber { get; set; } = string.Empty;
        public List<BankAccount> BankAccounts { get; set; }
    }
}
