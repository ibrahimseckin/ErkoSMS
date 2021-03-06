using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess.Interfaces
{
    public interface IExporter
    {
        int? Id { get; set; }
        string Name { get; set; }
        string Address { get; set; }
        string TradeRegisterNo { get; set; }
        string VatNo { get; set; }
        string PhoneNumber { get; set; }
        string FaxNumber { get; set; }
        List<BankAccount> BankAccounts { get; set; }
    }

    public class BankAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AccountDetails { get; set; }

    }
}
