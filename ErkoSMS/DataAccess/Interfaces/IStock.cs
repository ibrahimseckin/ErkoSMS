using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess.Interfaces
{
    public interface IStock
    {
         int Id { get; set; }
         IProduct Product { get; set; }
         int Reserved { get; set; }
    }
}
