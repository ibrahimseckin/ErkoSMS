using ErkoSMS.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ErkoSMS.ViewModels
{
    public class CustomerViewModel : ICustomer
    {
        public int Id { get; set; }

        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Display(Name = "Yorum")]
        public string Comment { get; set; }

        [Display(Name = "Sahibi")]
        public string Owner { get; set; }

        [Display(Name = "Sahibinin Telefonu")]
        public string OwnerMobile { get; set; }

        [Display(Name = "Sahibinin Email Adresi")]
        public string OwnerMail { get; set; }

        [Display(Name = "Yetkili")]
        public string Manager { get; set; }

        [Display(Name = "Yetkilinin Telefonu")]
        public string ManagerMobile { get; set; }

        [Display(Name = "Yetkilinin Email adresi")]
        public string ManagerEmail { get; set; }

        [Display(Name = "Yetkilinin Ünvanı")]
        public string ManagerTitle { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Şehir")]
        public string City { get; set; }

        [Display(Name = "Ülke")]
        public string Country { get; set; }

        [Display(Name = "Posta Kodu")]
        public string PostalCode { get; set; }

        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Ülke Kodu")]
        public string CountryCode { get; set; }

        [Display(Name = "Faks")]
        public string FaxNumber { get; set; }

        [Display(Name = "Durum")]
        public string Condition { get; set; }

        [Display(Name = "İletişim Kaynağı")]
        public string CommunicationMethod { get; set; }


        public DateTime? StartDate { get; set; }

        [Display(Name = "Firma İlgili Personel")]
        public string ContactPerson { get; set; }

        [Display(Name = "Vergi Ofisi")]
        public string TaxOffice { get; set; }

        [Display(Name = "Vergi Numarası")]
        public string TaxNumber { get; set; }

        [Display(Name = "Para Birimi")]
        public string Currency { get; set; }

        [Display(Name = "Bölge Kıta")]
        public string Region { get; set; }

        [Display(Name = "İndirim Oranı")]
        public double DiscountRate { get; set; }

        [Display(Name = "Satış Temsilcisi")]
        public string SalesRepresentative { get; set; }

        public CustomerViewModel()
        {

        }

        public CustomerViewModel(ICustomer customer)
        {
            Address = customer.Address;
            City = customer.City;
            Comment = customer.Comment;
            CommunicationMethod = customer.CommunicationMethod;
            Condition = customer.Condition;
            ContactPerson = customer.ContactPerson;
            Country = customer.Country;
            CountryCode = customer.CountryCode;
            Currency = customer.Currency;
            DiscountRate = customer.DiscountRate;
            FaxNumber = customer.FaxNumber;
            Id = customer.Id;
            Manager = customer.Manager;
            ManagerEmail = customer.ManagerEmail;
            ManagerMobile = customer.ManagerMobile;
            ManagerTitle = customer.ManagerTitle;
            Name = customer.Name;
            Owner = customer.Owner;
            OwnerMail = customer.OwnerMail;
            OwnerMobile = customer.OwnerMobile;
            PhoneNumber = customer.PhoneNumber;
            PostalCode = customer.PostalCode;
            Region = customer.Region;
            SalesRepresentative = customer.SalesRepresentative;
            StartDate = customer.StartDate;
            TaxNumber = customer.TaxNumber;
            TaxOffice = customer.TaxOffice;         
        }
    }
}