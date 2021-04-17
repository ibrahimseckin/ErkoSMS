using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNet.Identity.SQLite;
using ErkoSMS.DataAccess.Model;

namespace ErkoSMS.DataAccess
{
    public class SalesDataService
    {
        private IDataProvider _sqliteDataProvider;
        public SalesDataService()
        {
            _sqliteDataProvider = new SqliteDataProvider();
        }

        public IList<Sales> GetSalesBySalesPerson(string salesPeopleGuid)
        {
            const string query = "Select * From sales Where salespeople = @salesPeopleGuid";
            _sqliteDataProvider.AddParameter("@salesPeopleGuid", salesPeopleGuid);
            var dataSet = _sqliteDataProvider.ExecuteDataSet(query);
            IList<Sales> sales = new List<Sales>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                sales.Add(CreateSales(row));
            }

            return sales;
        }

        private Sales CreateSales(DataRow row)
        {
            return new Sales
            {
                Id = Convert.ToInt32(row["id"]),
                Currency = (Currency)Convert.ToInt32(row["currency"]),
                SalesState = (SalesState)Convert.ToInt32(row["state"]),
                TotalPrice = Convert.ToDouble(row["totalprice"]),
                Customer = GetCustomerByName(row["customername"].ToString()),
                InvoiceDate = (DateTime)(row["invoicedate"]),
                InvoiceNumber = row["invoicenumber"]?.ToString(),
                LastModifiedDate = (DateTime)(row["lastmodifieddate"]),
                SalesStartDate = (DateTime)(row["startdate"]),
                SalesUser = GetUserByGuid(row["salespeople"]?.ToString())
            };
        }

        private IdentityUser GetUserByGuid(string userGuid)
        {
            const string query = "Select * From aspnetusers Where id = @userguid";
            _sqliteDataProvider.AddParameter("@userguid", userGuid);
            DataRow row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            return CreateUser(row);
        }

        private IdentityUser CreateUser(DataRow row)
        {
            return new IdentityUser
            {
                Id = row["id"].ToString(),
                UserName = row["username"].ToString()
            };
        }

        private Customer GetCustomerByName(string customerName)
        {
            const string query = "Select * From customers Where name = @customername";
            _sqliteDataProvider.AddParameter("@customername", customerName);
            DataRow row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            return CreateCustomer(row);
        }

        private Customer CreateCustomer(DataRow row)
        {
            return new Customer
            {
                Id = Convert.ToInt32(row["id"]),
                Name = row["name"]?.ToString(),
                Comment = row["comment"]?.ToString(),
                Owner = row["owner"]?.ToString(),
                OwnerMobile = row["ownermobile"]?.ToString(),
                OwnerMail = row["ownermail"]?.ToString(),
                Manager = row["manager"]?.ToString(),
                ManagerMobile = row["managermobile"]?.ToString(),
                ManagerEmail = row["managermobile"]?.ToString(),
                ManagerTitle = row["managertitle"]?.ToString(),
                Address = row["adress"]?.ToString(),
                City = row["city"]?.ToString(),
                Country = row["country"]?.ToString(),
                PostalCode = row["postalcode"]?.ToString(),
                PhoneNumber = row["phonenumber"]?.ToString(),
                CountryCode = row["countrycode"]?.ToString(),
                FaxNumber = row["faxnumber"]?.ToString(),
                Condition = row["condition"]?.ToString(),
                CommunicationMethod = row["communicationmethod"]?.ToString(),
                StartDate = (DateTime)row["startdate"],
                ContactPerson = row["contactperson"]?.ToString(),
                TaxOffice = row["taxoffice"]?.ToString(),
                TaxNumber = row["taxnumber"]?.ToString(),
                Currency = row["currency"]?.ToString(),
                Region = row["region"]?.ToString(),
                DiscountRate = Convert.ToDouble(row["discountrate"]),
                SalesRepresentative = row["salesrepresentative"]?.ToString(),
            };
        }
    }
}
