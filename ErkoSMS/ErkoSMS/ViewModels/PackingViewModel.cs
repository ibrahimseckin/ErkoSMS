using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ErkoSMS.ViewModels
{
    public class PackingViewModel
    {
        public int OrderId { get; set; }
        public IList<PackingPallet> Pallets { get; set; }

        public class PackingPallet
        {
            public int PalletId { get; set; }
            public IList<PackedProduct> Products { get; set; }

            public class PackedProduct
            {
                public int Quantity { get; set; }
                public string ProductCode { get; set; }
            }
        }
    }
}