using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess
{
    public class MsSqlDataProvider : DataProvider<SqlDataAdapter, SqlParameter>
    {

        public MsSqlDataProvider()
        {

        }
        public override IDbConnection GetConnection()
        {
            const string key = "orkaConnectionString";
            string orkaConnectionString = ConfigurationManager.AppSettings[key];

            IDbConnection connection = new SqlConnection
            {
                ConnectionString = orkaConnectionString
            };

            return connection;
        }
    }
}
