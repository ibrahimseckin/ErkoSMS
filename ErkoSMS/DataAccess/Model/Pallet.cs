using System.ComponentModel.DataAnnotations;
using ErkoSMS.DataAccess.Interfaces;

namespace ErkoSMS.DataAccess.Model
{
    public class Pallet : IPallet
    {
        public Pallet()
        {

        }

        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public int Weight { get; set; }
        public int GrossWeight { get; set; }
        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Display(Name = "Açıklama (İngilizce)")]
        public string EnglishDescription { get; set; }

    }
}