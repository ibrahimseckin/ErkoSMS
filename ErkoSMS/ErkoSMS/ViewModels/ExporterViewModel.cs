using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ErkoSMS.DataAccess.Interfaces;
using ErkoSMS.DataAccess.Model;

namespace ErkoSMS.ViewModels
{
    public class ExporterViewModel : IExporter
    {
        public int? Id { get; set; }

        [Display(Name = "İthalatçı")]
        public string Name { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Ticaret Sicil Numarası")]
        public string TradeRegisterNo { get; set; }

        [Display(Name = "VAT No")]
        public string VatNo { get; set; }

        [Display(Name = "Telefon No")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Fax No")]
        public string FaxNumber { get; set; }

        public ExporterViewModel()
        {

        }

        public ExporterViewModel(Exporter exporter)
        {
            Id = exporter.Id;
            Name = exporter.Name;
            Address = exporter.Address;
            TradeRegisterNo= exporter.TradeRegisterNo;
            PhoneNumber = exporter.PhoneNumber;
            VatNo = exporter.VatNo;
            FaxNumber = exporter.FaxNumber;
        }
    }
}