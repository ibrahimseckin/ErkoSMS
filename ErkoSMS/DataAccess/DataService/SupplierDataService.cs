using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErkoSMS.DataAccess.Model;

namespace ErkoSMS.DataAccess
{
    public class SupplierDataService
    {
        private IDataProvider _sqliteDataProvider;
        public SupplierDataService()
        {
            _sqliteDataProvider = new SqliteDataProvider();
        }
        public IEnumerable<Supplier> GetAllSuppliers()
        {
            const string query = "select * from suppliers";
            var dataset = _sqliteDataProvider.ExecuteDataSet(query);
            IList<Supplier> suppliers = new List<Supplier>();
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                suppliers.Add(CreateSupplier(row));
            }

            return suppliers;
        }

        public Supplier GetSupplierById(int supplierId)
        {
            const string query = "Select * From suppliers Where Id = @supplierId";
            _sqliteDataProvider.AddParameter("@supplierId", supplierId);
            DataRow row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            return CreateSupplier(row);
        }

        public bool CreateSupplier(Supplier supplier)
        {
            const string query = "Insert into Suppliers (Name,Address,Country,PhoneNumber) values " +
                                 "(@Name,@Address,@Country,@PhoneNumber);" +
                                 "select last_insert_rowid();";
            _sqliteDataProvider.AddParameter("@Name", supplier.Name);
            _sqliteDataProvider.AddParameter("@Address", supplier.Address);
            _sqliteDataProvider.AddParameter("@Country", supplier.Country);
            _sqliteDataProvider.AddParameter("@PhoneNumber", supplier.PhoneNumber);
            var queryResult = _sqliteDataProvider.ExecuteScalar(query);
            return queryResult != null;
        }

        public bool UpdateSupplier(Supplier supplier)
        {
            const string query = "Update Suppliers " +
                                 "Set Name = @Name, Address = @Address," +
                                 "Country = @Country, PhoneNumber = @PhoneNumber " +
                                 " Where Id = @Id";
            _sqliteDataProvider.AddParameter("@Id", supplier.SupplierId);
            _sqliteDataProvider.AddParameter("@Name", supplier.Name);
            _sqliteDataProvider.AddParameter("@Address", supplier.Address);
            _sqliteDataProvider.AddParameter("@Country", supplier.Country);
            _sqliteDataProvider.AddParameter("@PhoneNumber", supplier.PhoneNumber);
            return _sqliteDataProvider.ExecuteNonQuery(query) > 0;
        }

        private Supplier CreateSupplier(DataRow row)
        {
            return new Supplier()
            {
                SupplierId = Convert.ToInt32(row["Id"]),
                Name = row["Name"].ToString(),
                Address = row["Address"]?.ToString(),
                Country = row["Country"]?.ToString(),
                PhoneNumber = row["PhoneNumber"]?.ToString()
            };
        }
    }
}
