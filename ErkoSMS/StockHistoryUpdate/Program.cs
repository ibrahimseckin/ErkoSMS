using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErkoSMS;

namespace StockHistoryUpdate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Stock history update operation is started");
            var stockHistoryHelper = new StockHistoryHelper();
            stockHistoryHelper.UpdateStockHistory();
            Console.WriteLine("Stock history update operation is completed");
        }
    }
}
