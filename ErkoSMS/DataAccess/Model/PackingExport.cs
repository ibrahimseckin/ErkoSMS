using System.Collections.Generic;

namespace ErkoSMS.DataAccess.Model
{
    public class PackedProductExport
    {
        public string PalletId { get; set; }
        public string Dimensions { get; set; }
        public string Quantity { get; set; }
        public string NetKG { get; set; }
        public string GrossKG { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
    }
}