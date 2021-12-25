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
            query += Environment.NewLine + "SELECT LAST_INSERT_ROWID();";
            _sqliteDataProvider.AddParameter("@name", exporter.Name);
            _sqliteDataProvider.AddParameter("@address", exporter.Address);
            _sqliteDataProvider.AddParameter("@tradeRegisterNo", exporter.TradeRegisterNo);
            _sqliteDataProvider.AddParameter("@vatNo", exporter.VatNo);
            _sqliteDataProvider.AddParameter("@PhoneNumber", exporter.PhoneNumber);
            _sqliteDataProvider.AddParameter("@FaxNumber", exporter.FaxNumber);
            var exporterId = Convert.ToInt32(_sqliteDataProvider.ExecuteScalar(query));
            foreach (var bankAccount in exporter.BankAccounts)
            {
                CreateBankAccount(bankAccount, exporterId);

            }
            return true;
        }

        private void CreateBankAccount(BankAccount bankAccount, int exporterId)
        {
            string query = "Insert into ExporterBankAccounts (ExporterId,Name,AccountDetails) values (@exporterid,@name,@accountdetails)";
            _sqliteDataProvider.AddParameter("@exporterid", exporterId);
            _sqliteDataProvider.AddParameter("@name", bankAccount.Name);
            _sqliteDataProvider.AddParameter("@accountdetails", bankAccount.AccountDetails);
            _sqliteDataProvider.ExecuteScalar(query);
        }


        public bool UpdateExporter(IExporter exporter)
        {
            string query = "Update Exporters Set name=@name, address=@address, " +
                                             "tradeRegisterNo=@tradeRegisterNo," +
                                             "vatNo=@vatNo,PhoneNumber=@PhoneNumber," +
                                             "FaxNumber=@FaxNumber where id = @id";
            _sqliteDataProvider.AddParameter("@id", exporter.Id);
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
            if (row != null)
            {
                var exporter = CreateExporterObject(row);
                exporter.BankAccounts = GetBankAccounts(exporter.Id);
                return exporter;
            }
            return null;
        }


        private List<BankAccount> GetBankAccounts(int? exporterId)
        {
            const string query = "Select * from ExporterBankAccounts where ExporterId = @exporterid";
            _sqliteDataProvider.AddParameter("@exporterid", exporterId);
            DataSet dataSet = _sqliteDataProvider.ExecuteDataSet(query);
            var bankAccounts = new List<BankAccount>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                bankAccounts.Add(new BankAccount()
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    AccountDetails = row["AccountDetails"]?.ToString()
                });
            }

            return bankAccounts;
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
