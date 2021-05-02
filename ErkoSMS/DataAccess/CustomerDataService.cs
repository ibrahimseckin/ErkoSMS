using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErkoSMS.DataAccess.Model;

namespace ErkoSMS.DataAccess
{
    public class CustomerDataService
    {
        private IDataProvider _sqliteDataProvider;
        public CustomerDataService()
        {
            _sqliteDataProvider = new SqliteDataProvider();
        }

        public IList<Customer> GetAllCustomers()
        {
            const string query = "select * from customers";
            var dataset = _sqliteDataProvider.ExecuteDataSet(query);
            IList<Customer> customers = new List<Customer>();
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                customers.Add(new Customer { Name = row["Name"].ToString(), Id = Convert.ToInt32(row["Id"]) });
            }

            return customers;
        }
    }
}
