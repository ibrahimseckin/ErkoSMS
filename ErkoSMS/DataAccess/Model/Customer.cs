using System;

namespace ErkoSMS.DataAccess.Model
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Owner { get; set; }
        public string OwnerMobile { get; set; }
        public string OwnerMail { get; set; }
        public string Manager { get; set; }
        public string ManagerMobile { get; set; }
        public string ManagerEmail { get; set; }
        public string ManagerTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }
        public string FaxNumber { get; set; }
        public string Condition { get; set; }
        public string CommunicationMethod { get; set; }
        public DateTime StartDate { get; set; }
        public string ContactPerson { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        public string Currency { get; set; }
        public string Region { get; set; }
        public double DiscountRate { get; set; }
        public string SalesRepresentative { get; set; }

    }
}
