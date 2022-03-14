using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess.Model
{
    public enum StockChangeState
    {
        [Display(Name = "Stok Arttı")]
        StokIncreased = 0,
        [Display(Name = "Stok Azaldı")]
        StokDecreased = 1
    }
}
