using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess
{
    public class ProductDataService
    {
        private IDataProvider _sqliteDataProvider;
        public ProductDataService()
        {
            _sqliteDataProvider = new SqliteDataProvider();
        }

    }
}
