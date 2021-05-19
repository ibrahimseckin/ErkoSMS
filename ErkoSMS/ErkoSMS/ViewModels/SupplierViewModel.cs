using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ErkoSMS.DataAccess.Interfaces;
using ErkoSMS.DataAccess.Model;

namespace ErkoSMS.ViewModels
{
    public class SupplierViewModel
    {
        public int SupplierId { get; set; }

        [Display(Name = "Tedarikçi")]
        public string Name { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Ülke")]
        public string Country { get; set; }

        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; }


        public SupplierViewModel()
        {

        }

        public SupplierViewModel(Supplier supplier)
        {
            SupplierId = supplier.SupplierId;
            Name = supplier.Name;
            Address = supplier.Address;
            Country= supplier.Country;
            PhoneNumber = supplier.PhoneNumber;
        }
    }
}