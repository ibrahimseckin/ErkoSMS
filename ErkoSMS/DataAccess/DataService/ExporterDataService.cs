using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ErkoSMS.DataAccess.Interfaces;
using ErkoSMS.DataAccess.Model;

namespace ErkoSMS.DataAccess.DataService
{
    public class ExporterDataService
    {
        private IDataProvider _sqliteDataProvider;
        public ExporterDataService()
        {
            _sqliteDataProvider = new SqliteDataProvider();
        }

        public bool CreateExporter(IExporter exporter)
        {
            string query = "Insert into Exporters (name,address,tradeRegisterNo,vatNo,PhoneNumber,FaxNumber) values (@name,@address," +
                                      "@tradeRegisterNo,@vatNo,@PhoneNumber,@FaxNumber);";
            _sqliteDataProvider.AddParameter("@name", exporter.Name);
            _sqliteDataProvider.AddParameter("@address", exporter.Address);
            _sqliteDataProvider.AddParameter("@tradeRegisterNo", exporter.TradeRegisterNo);
            _sqliteDataProvider.AddParameter("@vatNo", exporter.VatNo);
            _sqliteDataProvider.AddParameter("@PhoneNumber", exporter.PhoneNumber);
            _sqliteDataProvider.AddParameter("@FaxNumber", exporter.FaxNumber);
            var queryResult = _sqliteDataProvider.ExecuteScalar(query);
            return queryResult != null;
        }

        public bool UpdateExporter(IExporter exporter)
        {
            string query = "Update Exporters Set name=@name, address=@address, " +
                                             "tradeRegisterNo=@tradeRegisterNo," +
                                             "vatNo=@vatNo,PhoneNumber=@PhoneNumber," +
                                             "FaxNumber=@FaxNumber";
            _sqliteDataProvider.AddParameter("@name", exporter.Name);
            _sqliteDataProvider.AddParameter("@address", exporter.Address);
            _sqliteDataProvider.AddParameter("@tradeRegisterNo", exporter.TradeRegisterNo);
            _sqliteDataProvider.AddParameter("@vatNo", exporter.VatNo);
            _sqliteDataProvider.AddParameter("@PhoneNumber", exporter.PhoneNumber);
            _sqliteDataProvider.AddParameter("@FaxNumber", exporter.FaxNumber);
            return _sqliteDataProvider.ExecuteNonQuery(query) > 0;

        }

        public bool DeleteExporter(int id)
        {
            const string query = "Delete From Exporters Where Id = @id";
            _sqliteDataProvider.AddParameter("@id", id);
            return _sqliteDataProvider.ExecuteNonQuery(query) > 0;
        }

        public List<Exporter> GetAllExporters()
        {
            const string query = "select * from Exporters";
            var dataset = _sqliteDataProvider.ExecuteDataSet(query);
            var exporters = new List<Exporter>();
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                exporters.Add(CreateExporterObject(row));
            }

            return exporters;
        }

        public Exporter GetExporter(int id)
        {
            const string query = "select * from Exporters where id=@id";
            _sqliteDataProvider.AddParameter("@id", id);
            DataRow row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            return row != null ? CreateExporterObject(row) : null;
        }

        private Exporter CreateExporterObject(DataRow row)
        {
            return new Exporter
            {
                Id = Convert.ToInt32(row["id"]),
                Address = row["address"].ToString(),
                Name = row["name"].ToString(),
                TradeRegisterNo = row["tradeRegisterNo"].ToString(),
                VatNo = row["vatNo"].ToString(),
                FaxNumber = row["FaxNumber"].ToString(),
                PhoneNumber = row["PhoneNumber"].ToString()
            };
        }

    }
}
