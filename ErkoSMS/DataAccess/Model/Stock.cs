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
        public IProduct Product { get; set; }
        public int Reserved { get; set; }
    }
}
