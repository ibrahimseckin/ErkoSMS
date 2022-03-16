using ErkoSMS.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess.Model
{
    public class Stock : IStock
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int StockAmount { get; set; }
        public int ReservedAmount { get; set; }
        public double Price { get; set; }
    }
}
