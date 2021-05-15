using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess.Interfaces
{
    public interface IStockORKA
    {
        string Code { get; set; }
        int RemainingAmount { get; set; }
        double Price { get; set; }
    }
}
