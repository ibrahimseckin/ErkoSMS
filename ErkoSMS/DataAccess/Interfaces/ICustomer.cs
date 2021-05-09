using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess.Interfaces
{
    public interface ICustomer
    {
        int Id { get; set; }
        string Name { get; set; }
        string Comment { get; set; }
        string Owner { get; set; }
        string OwnerMobile { get; set; }
        string OwnerMail { get; set; }
        string Manager { get; set; }
        string ManagerMobile { get; set; }
        string ManagerEmail { get; set; }
        string ManagerTitle { get; set; }
        string Address { get; set; }
        string City { get; set; }
        string Country { get; set; }
        string PostalCode { get; set; }
        string PhoneNumber { get; set; }
        string CountryCode { get; set; }
        string FaxNumber { get; set; }
        string Condition { get; set; }
        string CommunicationMethod { get; set; }
        DateTime? StartDate { get; set; }
        string ContactPerson { get; set; }
        string TaxOffice { get; set; }
        string TaxNumber { get; set; }
        string Currency { get; set; }
        string Region { get; set; }
        double DiscountRate { get; set; }
        string SalesRepresentative { get; set; }
    }
}
