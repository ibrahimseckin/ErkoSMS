using ErkoSMS.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ErkoSMS.DataAccess.Interfaces;
using ErkoSMS.Models;

namespace ErkoSMS.ViewModels
{
    public class PalletViewModel : IPallet
    {
        public PalletViewModel()
        {

        }

        public PalletViewModel(IPallet pallet)
        {
            Id = pallet.Id;
            Width = pallet.Width;
            Height = pallet.Height;
            Depth = pallet.Depth;
            Weight = pallet.Weight;
            GrossWeight = pallet.GrossWeight;
            Description = pallet.Description;
            EnglishDescription = pallet.EnglishDescription;
        }

        public int Id { get; set; }
        [Display(Name = "Genişlik")]
        public int Width { get; set; }
        [Display(Name = "Yükseklik")]
        public int Height { get; set; }
        [Display(Name = "Derinlik")]
        public int Depth { get; set; }
        [Display(Name = "KG")]
        public int Weight { get; set; }
        [Display(Name = "Brüt KG")]
        public int GrossWeight { get; set; }
        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Display(Name = "Açıklama (İngilizce)")]
        public string EnglishDescription { get; set; }

    }
}