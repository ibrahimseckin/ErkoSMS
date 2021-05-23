using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess
{
    public class SqliteDataProvider : DataProvider<SQLiteDataAdapter,SQLiteParameter>
    {
        private static readonly string SqlLiteSourcePath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
        private static readonly string SqlLiteFileName = "ErkoSMS.db";
        private static readonly string SqlLiteDataSource = Path.Combine(SqlLiteSourcePath, SqlLiteFileName);

        public override IDbConnection GetConnection()
        {
            IDbConnection connection = new SQLiteConnection
            {
                ConnectionString = $"Data Source={SqlLiteDataSource};foreign keys=true"
            };
            return connection;
        }
    }
}
