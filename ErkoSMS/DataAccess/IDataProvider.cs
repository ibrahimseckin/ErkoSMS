using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkoSMS.DataAccess
{
    interface IDataProvider
    {
        int ExecuteNonQuery(string commandText);

        int ExecuteNonQuery(string commandText, CommandType commandType);


        object ExecuteScalar(string commandText);

        object ExecuteScalar(string commandText, CommandType commandType);

        DataSet ExecuteDataSet(string commandText);

        DataSet ExecuteDataSet(string commandText, CommandType commandType);

        IEnumerable<DataRow> ExecuteDataRows(string commandText);

        IEnumerable<DataRow> ExecuteDataRows(string commandText, CommandType commandType);
    }
}
