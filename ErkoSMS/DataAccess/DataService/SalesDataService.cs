using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
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


        public IList<Sales> GetSales(DateTime startDate, DateTime endDate)
        {
            const string query = "SELECT * From sales Where StartDate BETWEEN @StartDate AND @EndDate ";
            _sqliteDataProvider.AddParameter("@StartDate", startDate);
            _sqliteDataProvider.AddParameter("@EndDate", endDate);
            var dataSet = _sqliteDataProvider.ExecuteDataSet(query);
            IList<Sales> sales = new List<Sales>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                var sale = CreateSales(row);
                var saleId = Convert.ToInt32(row["Id"].ToString());
                sale.SalesDetails = GetSalesDetails(saleId);
                sales.Add(sale);
            }

            return sales;
        }

        private IList<SalesDetail> GetSalesDetails(int salesId)
        {
            const string salesDetailsQuery = "Select * from Sales_Product where SalesId = @salesid";
            _sqliteDataProvider.AddParameter("@salesid", salesId);
            DataSet dataSet = _sqliteDataProvider.ExecuteDataSet(salesDetailsQuery);
            var salesDetails = new List<SalesDetail>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                var productId = Convert.ToInt32(row["ProductId"]);
                var productCode = new ProductDataService().GetProductById(productId).Code;
                var quantity = Convert.ToInt32(row["Quantity"]);
                var unitPrice = Convert.ToDouble(row["UnitPrice"]);
                salesDetails.Add(new SalesDetail
                {
                    ProductCode = productCode,
                    Quantity = quantity,
                    SalesId = salesId,
                    UnitPrice = unitPrice
                });
            }

            return salesDetails;
        }

        public Sales GetSalesById(int id)
        {
            const string query = "SELECT * From sales Where id = @id";
            _sqliteDataProvider.AddParameter("@id", id);
            var dataRow = _sqliteDataProvider.ExecuteDataRows(query).First();
            var sales = CreateSales(dataRow);

            const string salesDetailsQuery = "Select * from Sales_Product where SalesId = @salesid";
            _sqliteDataProvider.AddParameter("@salesid", id);
            DataSet dataSet = _sqliteDataProvider.ExecuteDataSet(salesDetailsQuery);
            var salesDetails = new List<SalesDetail>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                var productId = Convert.ToInt32(row["ProductId"]);
                var productCode = new ProductDataService().GetProductById(productId).Code;
                var quantity = Convert.ToInt32(row["Quantity"]);
                var unitPrice = Convert.ToDouble(row["UnitPrice"]);
                salesDetails.Add(new SalesDetail
                {
                    ProductCode = productCode,
                    Quantity = quantity,
                    SalesId = id,
                    UnitPrice = unitPrice
                });
            }

            sales.SalesDetails = salesDetails;
            return sales;
        }

        public int CreateOrder(Sales sales)
        {
            string query = "Insert into Sales (CustomerName,SalesPeople,TotalPrice,Currency,State,InvoiceNumber,InvoiceDate,StartDate) values (@customerName,@salesPeopleGuid," +
                "@totalPrice,@currency,@state,@invoiceNumber,@invoiceDate,@salesStartDate);";
            query += Environment.NewLine + "SELECT LAST_INSERT_ROWID();";
            _sqliteDataProvider.AddParameter("@customerName",sales.Customer.Name);
            _sqliteDataProvider.AddParameter("@salesPeopleGuid", sales.SalesUserGuid);
            _sqliteDataProvider.AddParameter("@totalPrice", sales.TotalPrice);
            _sqliteDataProvider.AddParameter("@currency", sales.Currency);
            _sqliteDataProvider.AddParameter("@state", sales.SalesState);
            _sqliteDataProvider.AddParameter("@invoiceNumber", sales.InvoiceNumber);
            _sqliteDataProvider.AddParameter("@invoiceDate", sales.InvoiceDate);
            _sqliteDataProvider.AddParameter("@salesStartDate", DateTime.Now);

            var salesId = _sqliteDataProvider.ExecuteScalar(query);
            foreach (var salesDetail in sales.SalesDetails)
            {
                string salesDetailQuery = "Insert into Sales_Product (SalesId,ProductId,Quantity,UnitPrice) values (@salesid,@productid," +
                "@quantity,@unitprice);";
                _sqliteDataProvider.AddParameter("@salesid", salesId);
                var productId = new ProductDataService().GetProductByCode(salesDetail.ProductCode).Id;
                _sqliteDataProvider.AddParameter("@productid", productId);
                _sqliteDataProvider.AddParameter("@quantity", salesDetail.Quantity);
                _sqliteDataProvider.AddParameter("@unitprice", salesDetail.UnitPrice);
                _sqliteDataProvider.ExecuteScalar(salesDetailQuery);
            }

            return Convert.ToInt32(salesId);
        }

        public bool UpdateOrder(Sales sales)
        {
            string query = "Update Sales Set CustomerName = @customerName, State = @state, InvoiceNumber = @invoiceNumber, InvoiceDate = @invoiceDate, Currency = @currency, LastModifiedDate = @lastModifiedDate where id= @id;";
            _sqliteDataProvider.AddParameter("@id", sales.Id);
            _sqliteDataProvider.AddParameter("@customerName", sales.Customer.Name);
            _sqliteDataProvider.AddParameter("@state", sales.SalesState);
            _sqliteDataProvider.AddParameter("@invoiceNumber", sales.InvoiceNumber);
            _sqliteDataProvider.AddParameter("@invoiceDate", sales.InvoiceDate); 
            _sqliteDataProvider.AddParameter("@currency", sales.Currency);
            _sqliteDataProvider.AddParameter("@lastModifiedDate", sales.LastModifiedDate);
            return _sqliteDataProvider.ExecuteNonQuery(query) > 0;
        }

        public bool UpdateOrderState(int orderId, SalesState salesState)
        {
            string query = "Update Sales Set State = @salesState where id= @id;";
            _sqliteDataProvider.AddParameter("@id", orderId);
            _sqliteDataProvider.AddParameter("@salesState", salesState);
            return _sqliteDataProvider.ExecuteNonQuery(query) > 0;
        }

        public int GetReservedCountyProductId(int productId)
        {
            const string query = "SELECT quantity From sales s join sales_product sd on s.id = sd.salesid  " +
                                 "Where s.state != @salesState and lower(sd.productid) = lower(@productId)";
            _sqliteDataProvider.AddParameter("@salesState", SalesState.InvoiceDoneAndPacked);
            _sqliteDataProvider.AddParameter("@productid", productId);
            var dataSet = _sqliteDataProvider.ExecuteDataSet(query);
            int reserved = 0;
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                reserved += Convert.ToInt32(row["quantity"].ToString());
            }

            return reserved;
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
            var SalesUserGuid = row["salespeople"]?.ToString();

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
                SalesUserGuid = SalesUserGuid
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
