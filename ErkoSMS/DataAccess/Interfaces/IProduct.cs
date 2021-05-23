using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess.Interfaces
{
    public interface IProduct
    {
        int Id { get; set; }
        string Code { get; set; }
       string CrossReferenceCode { get; set; }
       string Description { get; set; }
       string EnglishDescription { get; set; }
       string Group { get; set; }
       string Brand { get; set; }
       string Model { get; set; }
       double LastPrice { get; set; }
    }
}
