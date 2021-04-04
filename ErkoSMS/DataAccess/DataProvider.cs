﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ErkoSMS.DataAccess
{

    public abstract class DataProvider<TAdapter, TDataParameter>
        where TAdapter : IDbDataAdapter, new()
        where TDataParameter : IDbDataParameter, new()
    {
        private static readonly string SqlLiteSourcePath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
        private static readonly string SqlLiteFileName = "ErkoSMS.db";
        private static readonly string SqlLiteDataSource = Path.Combine(SqlLiteSourcePath, SqlLiteFileName);

        private readonly List<TDataParameter> _parameters = new List<TDataParameter>();
        /// <summary>
        /// Overridden method for getting connection
        /// </summary>
        /// <returns>Returns the SQLite connection obejct as IDbConnection</returns>
        public IDbConnection GetConnection()
        {
            IDbConnection connection = new SQLiteConnection
            {
                ConnectionString = $"Data Source={SqlLiteDataSource};foreign keys=true"
            };
            return connection;
        }

        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, CommandType.Text);
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType)
        {
            var connection = GetConnection();
            var command = connection.CreateCommand();

            using (connection)
            {
                using (command)
                {
                    command.CommandType = commandType;
                    command.Connection = connection;
                    command.CommandText = commandText;
                    PrepareCommandParameters(command);
                    connection.Open();
                    int effectedRow = command.ExecuteNonQuery();
                    _parameters.Clear();
                    return effectedRow;
                }
            }
        }

        public object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(commandText, CommandType.Text);
        }
        public object ExecuteScalar(string commandText, CommandType commandType)
        {
            var connection = GetConnection();
            var command = connection.CreateCommand();

            using (connection)
            {
                using (command)
                {
                    command.CommandType = commandType;
                    command.Connection = connection;
                    command.CommandText = commandText;
                    PrepareCommandParameters(command);
                    connection.Open();
                    var returnValue = command.ExecuteScalar();
                    _parameters.Clear();
                    return returnValue;

                }
            }
        }

        public DataSet ExecuteDataSet(string commandText)
        {
            return ExecuteDataSet(commandText, CommandType.Text);
        }
        public DataSet ExecuteDataSet(string commandText, CommandType commandType)
        {
            var connection = GetConnection();
            var command = connection.CreateCommand();

            using (connection)
            {
                using (command)
                {
                    command.CommandType = commandType;
                    command.Connection = connection;
                    command.CommandText = commandText;
                    PrepareCommandParameters(command);
                    connection.Open();
                    TAdapter dataAdapter = new TAdapter {SelectCommand = command};
                    DataSet dataSet = new DataSet { Locale = CultureInfo.InvariantCulture };
                    dataAdapter.Fill(dataSet);
                    _parameters.Clear();
                    return dataSet;
                }
            }
        }

        public IEnumerable<DataRow> ExecuteDataRows(string commandText)
        {
            return ExecuteDataRows(commandText, CommandType.Text);
        }
        public IEnumerable<DataRow> ExecuteDataRows(string commandText, CommandType commandType)
        {
            return ExecuteDataSet(commandText, commandType).Tables.OfType<DataTable>().FirstOrDefault()?.Rows.OfType<DataRow>();
        }

        private void PrepareCommandParameters(IDbCommand command)
        {
            foreach (var param in _parameters)
            {
                command.Parameters.Add(param);
            }
        }
    }
}