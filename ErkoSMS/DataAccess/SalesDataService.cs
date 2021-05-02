﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
            const string query = "SELECT * From sales Where salespeople = @salesPeopleGuid";
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

            var Id = Convert.ToInt32(row["Id"]);
            var Currency = (Currency)Convert.ToInt32(row["currency"]);
            var SalesState = DBNull.Value.Equals(row["state"])
                ? Model.SalesState.Nonspecified
                : (SalesState)Convert.ToInt32(row["state"]);
            var TotalPrice = Convert.ToDouble(row["totalprice"]);
            var Customer = GetCustomerByName(row["customername"].ToString());
            var InvoiceDate = string.IsNullOrEmpty(row["InvoiceDate"].ToString()) ? (DateTime?)null : DateTime.Parse(row["InvoiceDate"].ToString());
            var InvoiceNumber = row["InvoiceNumber"]?.ToString();
            var LastModifiedDate = string.IsNullOrEmpty(row["lastmodifieddate"].ToString()) ? (DateTime?)null : DateTime.Parse(row["lastmodifieddate"].ToString());
            var SalesStartDate = DateTime.Parse(row["startdate"].ToString());
            var SalesUser = GetUserByGuid(row["salespeople"]?.ToString());

            return new Sales
            {
                Id = Id,
                Currency = Currency,
                SalesState = SalesState,
                TotalPrice = TotalPrice,
                Customer = Customer,
                InvoiceDate = InvoiceDate,
                InvoiceNumber = InvoiceNumber,
                LastModifiedDate = LastModifiedDate,
                SalesStartDate = SalesStartDate,
                SalesUser = SalesUser
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
                Id = row["Id"].ToString(),
                UserName = row["username"].ToString()
            };
        }

        private Customer GetCustomerByName(string customerName)
        {
            const string query = "Select * From customers Where name = @customername";
            _sqliteDataProvider.AddParameter("@customername", customerName);
            DataRow row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            if (row == null)
            {
                return new Customer {Name = customerName};
            }
            else
            {
                return CreateCustomer(row);
            }
            
        }

        private Customer CreateCustomer(DataRow row)
        {
            var Id = Convert.ToInt32(row["Id"]);
            var Name = row["name"]?.ToString();
            var Comment = row["comment"]?.ToString();
            var Owner = row["owner"]?.ToString();
            var OwnerMobile = row["ownermobile"]?.ToString();
            var OwnerMail = row["ownermail"]?.ToString();
            var Manager = row["manager"]?.ToString();
            var ManagerMobile = row["managermobile"]?.ToString();
            var ManagerEmail = row["managermobile"]?.ToString();
            var ManagerTitle = row["managertitle"]?.ToString();
            var Address = row["adress"]?.ToString();
            var City = row["city"]?.ToString();
            var Country = row["country"]?.ToString();
            var PostalCode = row["postalcode"]?.ToString();
            var PhoneNumber = row["phonenumber"]?.ToString();
            var CountryCode = row["countrycode"]?.ToString();
            var FaxNumber = row["faxnumber"]?.ToString();
            var Condition = row["condition"]?.ToString();
            var CommunicationMethod = row["communicationmethod"]?.ToString();
            DateTime? StartDate = string.IsNullOrEmpty(row["startdate"].ToString()) ? (DateTime?)null : DateTime.Parse(row["startdate"].ToString());
            var ContactPerson = row["contactperson"]?.ToString();
            var TaxOffice = row["taxoffice"]?.ToString();
            var TaxNumber = row["taxnumber"]?.ToString();
            var Currency = row["currency"]?.ToString();
            var Region = row["region"]?.ToString();
            var DiscountRate = Convert.ToDouble(row["discountrate"]);
            var SalesRepresentative = row["salesrepresentative"]?.ToString();

            return new Customer
            {
                Name = Name,
                Id = Id
            };
        }
    }
}
