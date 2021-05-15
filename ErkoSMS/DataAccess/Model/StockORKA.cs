using ErkoSMS.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess.Model
{
    public class StockORKA : IStockORKA
    {
        public string Code { get; set; }
        public int RemainingAmount { get; set; }
        public double Price { get; set; }
    }
}
