using ErkoSMS.DataAccess.Model;
using System.Collections.Generic;

namespace ErkoSMS
{
    /// <summary>
    /// This class represent of Alarm Module Operator Filter fields.
    /// </summary>
    public class OrderFilterParameters
    {
        public SalesState State { get; set; }
        public IEnumerable<string> Customers { get; set; } = new List<string>();
    }
}