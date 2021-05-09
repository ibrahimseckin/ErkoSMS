using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErkoSMS.DataAccess.Interfaces;
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
                customers.Add(CreateCustomerObject(row));
            }

            return customers;
        }


        public Customer GetCustomerByName(string customerName)
        {
            const string query = "select * from customers where Name=@name";
            _sqliteDataProvider.AddParameter("@name", customerName);
            DataRow row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            
            return row != null ? CreateCustomerObject(row) : null;
        }

        public bool DeleteCustomerById(int customerId)
        {
            const string query = "Delete From customers Where Id = @customerId";
            _sqliteDataProvider.AddParameter("@customerId", customerId);
            return _sqliteDataProvider.ExecuteNonQuery(query) > 0;
        }

        private Customer CreateCustomerObject(DataRow row)
        {
            var customer = new Customer
            {
                Id = Convert.ToInt32(row["Id"]),
                Address = row["Adress"].ToString(),
                City = row["City"].ToString(),
                Comment = row["Comment"].ToString(),
                CommunicationMethod = row["CommunicationMethod"].ToString(),
                Condition = row["Condition"].ToString(),
                ContactPerson = row["ContactPerson"].ToString(),
                Country = row["Country"].ToString(),
                CountryCode = row["CountryCode"].ToString(),
                Currency = row["Currency"].ToString(),
                DiscountRate = Convert.IsDBNull(row["DiscountRate"]) || string.IsNullOrEmpty(row["DiscountRate"].ToString()) ? 0 : Convert.ToDouble(row["DiscountRate"].ToString()),
                FaxNumber = row["FaxNumber"].ToString(),
                Manager = row["Manager"].ToString(),
                ManagerEmail = row["ManagerEmail"].ToString(),
                ManagerMobile = row["ManagerMobile"].ToString(),
                ManagerTitle = row["ManagerTitle"].ToString(),
                Name = row["Name"].ToString(),
                Owner = row["Owner"].ToString(),
                OwnerMail = row["OwnerMail"].ToString(),
                OwnerMobile = row["OwnerMobile"].ToString(),
                PhoneNumber = row["PhoneNumber"].ToString(),
                PostalCode = row["PostalCode"].ToString(),
                Region = row["Region"].ToString(),
                SalesRepresentative = row["SalesRepresentative"].ToString(),
                TaxNumber = row["TaxNumber"].ToString(),
                TaxOffice = row["TaxOffice"].ToString(),
            };

            if (Convert.IsDBNull(row["StartDate"]) == false && string.IsNullOrEmpty(row["StartDate"].ToString()) == false)
            {
                customer.StartDate = DateTime.Parse(row["StartDate"].ToString());
            }


            return customer;
        }

        public bool CreateCustomer(ICustomer customer)
        {
            const string query = "Insert into Customers (Name,Comment,Owner,OwnerMobile,OwnerMail,Manager,ManagerMobile,ManagerEmail,ManagerTitle," +
                "                 Adress,City,Country,PostalCode,PhoneNumber,CountryCode,FaxNumber,Condition,CommunicationMethod,StartDate,ContactPerson," +
                "                 TaxOffice,TaxNumber,Currency,Region,DiscountRate,SalesRepresentative) values " +
                                "(@Name,@Comment,@Owner,@OwnerMobile,@OwnerMail,@Manager,@ManagerMobile,@ManagerEmail,@ManagerTitle,@Adress,@City," +
                                "@Country,@PostalCode,@PhoneNumber,@CountryCode,@FaxNumber,@Condition,@CommunicationMethod,@StartDate,@ContactPerson," +
                                "@TaxOffice,@TaxNumber,@Currency,@Region,@DiscountRate,@SalesRepresentative);" +
                                "select last_insert_rowid();";
            _sqliteDataProvider.AddParameter("@Name", customer.Name);
            _sqliteDataProvider.AddParameter("@Comment", customer.Comment);
            _sqliteDataProvider.AddParameter("@Owner", customer.Owner);
            _sqliteDataProvider.AddParameter("@OwnerMobile", customer.OwnerMobile);
            _sqliteDataProvider.AddParameter("@OwnerMail", customer.OwnerMail);
            _sqliteDataProvider.AddParameter("@Manager", customer.Manager);
            _sqliteDataProvider.AddParameter("@ManagerMobile", customer.ManagerMobile);
            _sqliteDataProvider.AddParameter("@ManagerEmail", customer.ManagerEmail);
            _sqliteDataProvider.AddParameter("@ManagerTitle", customer.ManagerTitle);
            _sqliteDataProvider.AddParameter("@Adress", customer.Address);
            _sqliteDataProvider.AddParameter("@City", customer.City);
            _sqliteDataProvider.AddParameter("@Country", customer.Country);
            _sqliteDataProvider.AddParameter("@PostalCode", customer.PostalCode);
            _sqliteDataProvider.AddParameter("@PhoneNumber", customer.PhoneNumber);
            _sqliteDataProvider.AddParameter("@CountryCode", customer.CountryCode);
            _sqliteDataProvider.AddParameter("@FaxNumber", customer.FaxNumber);
            _sqliteDataProvider.AddParameter("@Condition", customer.Condition);
            _sqliteDataProvider.AddParameter("@CommunicationMethod", customer.CommunicationMethod);
            _sqliteDataProvider.AddParameter("@StartDate", DateTime.Today);
            _sqliteDataProvider.AddParameter("@ContactPerson", customer.ContactPerson);
            _sqliteDataProvider.AddParameter("@TaxOffice", customer.TaxOffice);
            _sqliteDataProvider.AddParameter("@TaxNumber", customer.TaxNumber);
            _sqliteDataProvider.AddParameter("@Currency", customer.Currency);
            _sqliteDataProvider.AddParameter("@Region", customer.Region);
            _sqliteDataProvider.AddParameter("@DiscountRate", customer.DiscountRate);
            _sqliteDataProvider.AddParameter("@SalesRepresentative", customer.SalesRepresentative);
            var queryResult = _sqliteDataProvider.ExecuteScalar(query);
            return queryResult != null;
        }
    }
}
