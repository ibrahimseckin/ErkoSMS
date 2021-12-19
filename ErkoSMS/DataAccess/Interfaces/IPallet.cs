using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess.Interfaces
{
    public interface IPallet
    {
        int Id { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        int Depth { get; set; }
        int Weight { get; set; }
        int GrossWeight { get; set; }
        string Description { get; set; }
        string EnglishDescription { get; set; }
    }
}
