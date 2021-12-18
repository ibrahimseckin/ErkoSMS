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

        public IList<Sales> GetAllSales()
        {
            const string query = "SELECT * From sales";
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
            string query = "Insert into Sales (CustomerName,SalesPeople,TotalPrice,Currency,State,InvoiceNumber,InvoiceDate,StartDate,ExchangeRate,ExporterId) values (@customerName,@salesPeopleGuid," +
                "@totalPrice,@currency,@state,@invoiceNumber,@invoiceDate,@salesStartDate,@exchangeRate,@exporterId);";
            query += Environment.NewLine + "SELECT LAST_INSERT_ROWID();";
            _sqliteDataProvider.AddParameter("@customerName", sales.Customer.Name);
            _sqliteDataProvider.AddParameter("@salesPeopleGuid", sales.SalesUserGuid);
            _sqliteDataProvider.AddParameter("@totalPrice", sales.TotalPrice);
            _sqliteDataProvider.AddParameter("@currency", sales.Currency);
            _sqliteDataProvider.AddParameter("@state", sales.SalesState);
            _sqliteDataProvider.AddParameter("@invoiceNumber", sales.InvoiceNumber);
            _sqliteDataProvider.AddParameter("@invoiceDate", sales.InvoiceDate);
            _sqliteDataProvider.AddParameter("@salesStartDate", DateTime.Now);
            _sqliteDataProvider.AddParameter("@exchangeRate", sales.ExchangeRate);
            _sqliteDataProvider.AddParameter("@exporterId", sales.Exporter?.Id);

            var salesId = Convert.ToInt32(_sqliteDataProvider.ExecuteScalar(query));
            foreach (var salesDetail in sales.SalesDetails)
            {
                CreateSalesDetails(salesDetail, salesId);

            }

            return salesId;
        }


        private void CreateSalesDetails(SalesDetail salesDetail, int salesId)
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


        public void UpdateOrder(Sales sales)
        {
            string query = "Update Sales Set CustomerName = @customerName, " +
                                        "State = @state, InvoiceNumber = @invoiceNumber, " +
                                        "InvoiceDate = @invoiceDate, Currency = @currency, " +
                                        "TotalPrice = @totalPrice,  LastModifiedDate = @lastModifiedDate, " +
                                        "ExchangeRate = @exchangeRate, ExporterId = @exporterId where id= @id;";
            _sqliteDataProvider.AddParameter("@id", sales.Id);
            _sqliteDataProvider.AddParameter("@customerName", sales.Customer.Name);
            _sqliteDataProvider.AddParameter("@state", sales.SalesState);
            _sqliteDataProvider.AddParameter("@invoiceNumber", sales.InvoiceNumber);
            _sqliteDataProvider.AddParameter("@invoiceDate", sales.InvoiceDate);
            _sqliteDataProvider.AddParameter("@currency", sales.Currency);
            _sqliteDataProvider.AddParameter("@totalPrice", sales.TotalPrice);
            _sqliteDataProvider.AddParameter("@lastModifiedDate", sales.LastModifiedDate);
            _sqliteDataProvider.AddParameter("@exchangeRate", sales.ExchangeRate);
            _sqliteDataProvider.AddParameter("@exporterId", sales.Exporter?.Id);
            _sqliteDataProvider.ExecuteNonQuery(query);

            DeleteOrderDetails(sales.Id);
            foreach (var salesDetail in sales.SalesDetails)
            {
                CreateSalesDetails(salesDetail, sales.Id);
            }
        }

        private void DeleteOrderDetails(int salesId)
        {
            const string query = "Delete From Sales_Product Where SalesId = @salesId";
            _sqliteDataProvider.AddParameter("@salesId", salesId);
            _sqliteDataProvider.ExecuteNonQuery(query);
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

        public IList<int> GetAllProductsForActiveOrders()
        {
            const string query = "SELECT sd.ProductId as productid From sales s join sales_product sd on s.id = sd.salesid  " +
                                 "Where s.state != @salesState";
            _sqliteDataProvider.AddParameter("@salesState", SalesState.InvoiceDoneAndPacked);
            var dataSet = _sqliteDataProvider.ExecuteDataSet(query);
            var productIds = new List<int>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                productIds.Add(Convert.ToInt32(row["productid"].ToString()));
            }

            return productIds;
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
            var SalesStartDate = string.IsNullOrEmpty(row["startdate"].ToString())
                ? DateTime.MinValue
                : DateTime.Parse(row["startdate"].ToString());
            var SalesUserGuid = row["salespeople"]?.ToString();
            var ExchangeRate = DBNull.Value.Equals(row["ExchangeRate"]) ? 0.0 : Convert.ToDouble(row["ExchangeRate"]);
            var Exporter = DBNull.Value.Equals(row["ExporterId"]) ? new Exporter() : GetExporterById(Convert.ToInt32(row["ExporterId"]));
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
                SalesUserGuid = SalesUserGuid,
                ExchangeRate = ExchangeRate,
                Exporter = Exporter
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
                return new Customer { Name = customerName };
            }
            else
            {
                return CreateCustomer(row);
            }

        }

        private Exporter GetExporterById(int exporterId)
        {
            const string query = "Select * From Exporters Where id = @id";
            _sqliteDataProvider.AddParameter("@id", exporterId);
            DataRow row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            if (row == null)
            {
                return new Exporter { Id = exporterId };
            }
            else
            {
                return CreateExporter(row);
            }

        }

        private Exporter CreateExporter(DataRow row)
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

        private Customer CreateCustomer(DataRow row)
        {
            return new Customer
            {
                Name = row["name"]?.ToString(),
                Id = Convert.ToInt32(row["Id"]),
                Currency = row["currency"]?.ToString(),
                Address = row["adress"]?.ToString(),
                City = row["city"]?.ToString(),
                Comment = row["comment"]?.ToString(),
                CommunicationMethod = row["communicationmethod"]?.ToString(),
                Condition = row["condition"]?.ToString(),
                ContactPerson = row["contactperson"]?.ToString(),
                Country = row["country"]?.ToString(),
                CountryCode = row["countrycode"]?.ToString(),
                DiscountRate = Convert.ToDouble(row["discountrate"]),
                FaxNumber = row["faxnumber"]?.ToString(),
                Manager = row["manager"]?.ToString(),
                ManagerEmail = row["managermobile"]?.ToString(),
                ManagerMobile = row["managermobile"]?.ToString(),
                ManagerTitle = row["managertitle"]?.ToString(),
                Owner = row["owner"]?.ToString(),
                OwnerMail = row["ownermail"]?.ToString(),
                OwnerMobile = row["ownermobile"]?.ToString(),
                PhoneNumber = row["phonenumber"]?.ToString(),
                PostalCode = row["postalcode"]?.ToString(),
                Region = row["region"]?.ToString(),
                SalesRepresentative = row["salesrepresentative"]?.ToString(),
                StartDate = string.IsNullOrEmpty(row["startdate"].ToString()) ? (DateTime?)null : DateTime.Parse(row["startdate"].ToString()),
                TaxNumber = row["taxnumber"]?.ToString(),
                TaxOffice = row["taxoffice"]?.ToString()
            };
        }

        public bool DeleteSales(int salesId)
        {
            const string query = "Delete From Sales Where Id = @salesId";
            _sqliteDataProvider.AddParameter("@salesId", salesId);
            return _sqliteDataProvider.ExecuteNonQuery(query) > 0;
        }
    }
}
