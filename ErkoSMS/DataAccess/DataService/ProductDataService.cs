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
    public class ProductDataService
    {
        private IDataProvider _sqliteDataProvider;
        public ProductDataService()
        {
            _sqliteDataProvider = new SqliteDataProvider();
        }

        public IList<Product> GetAllProducts()
        {
            const string query = "select * from products";
            var dataset = _sqliteDataProvider.ExecuteDataSet(query);
            IList<Product> products = new List<Product>();
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                products.Add(CreateProduct(row));
            }

            return products;
        }

        public Product GetProductById(int productId)
        {
            const string query = "Select * From products Where Id = @productid";
            _sqliteDataProvider.AddParameter("@productid", productId);
            DataRow row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            return CreateProduct(row);
        }

        public Product GetProductByCode(string productCode)
        {
            const string query = "Select * From products Where code = @productCode";
            _sqliteDataProvider.AddParameter("@productCode", productCode);
            DataRow row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            return row != null ? CreateProduct(row) : null;
        }


        public IList<Product> GetProductByCodeWithWildCard(string productCode)
        {
            const string query = "Select * From products Where code like @productCode";
            _sqliteDataProvider.AddParameter("@productCode", $"%{productCode}%");
            var rows = _sqliteDataProvider.ExecuteDataRows(query);
            IList<Product> products = new List<Product>();
            foreach (DataRow row in rows)
            {
                products.Add(CreateProduct(row));
            }

            return products;
        }

        public bool DeleteProductByCode(string productCode)
        {
            const string query = "Delete From products Where code = @productCode";
            _sqliteDataProvider.AddParameter("@productCode", productCode);
            return _sqliteDataProvider.ExecuteNonQuery(query) > 0;
        }

        public bool DeleteProductById(int productId)
        {
            const string query = "Delete From products Where Id = @productId";
            _sqliteDataProvider.AddParameter("@productId", productId);
            return _sqliteDataProvider.ExecuteNonQuery(query) > 0;
        }

        public bool CreateProduct(IProduct product)
        {
            const string query = "Insert into Products (Code,CrossReferenceCode,Description,DescriptionEng,ProductGroup,Brand,Model,LastPrice) values " +
                                "(@Code,@CrossReferenceCode,@Description,@DescriptionEng,@ProductGroup,@Brand,@Model,@LastPrice);" +
                                "select last_insert_rowid();";
            _sqliteDataProvider.AddParameter("@Code", product.Code);
            _sqliteDataProvider.AddParameter("@CrossReferenceCode", product.CrossReferenceCode);
            _sqliteDataProvider.AddParameter("@Description", product.Description);
            _sqliteDataProvider.AddParameter("@DescriptionEng", product.EnglishDescription);
            _sqliteDataProvider.AddParameter("@ProductGroup", product.Group);
            _sqliteDataProvider.AddParameter("@Brand", product.Brand);
            _sqliteDataProvider.AddParameter("@Model", product.Model);
            _sqliteDataProvider.AddParameter("@LastPrice", product.LastPrice);
            var queryResult = _sqliteDataProvider.ExecuteScalar(query);
            return queryResult != null;
        }

        public bool UpdateProduct(IProduct product)
        {
            const string query = "Update Products " +
                           "Set Code = @Code, CrossReferenceCode = @CrossReferenceCode," +
                           "Description = @Description, DescriptionEng = @DescriptionEng," +
                           "ProductGroup = @ProductGroup, Brand = @Brand, Model = @Model,LastPrice = @LastPrice " +
                           " Where Id = @Id";
            _sqliteDataProvider.AddParameter("@Id", product.Id);
            _sqliteDataProvider.AddParameter("@Code", product.Code);
            _sqliteDataProvider.AddParameter("@CrossReferenceCode", product.CrossReferenceCode);
            _sqliteDataProvider.AddParameter("@Description", product.Description);
            _sqliteDataProvider.AddParameter("@DescriptionEng", product.EnglishDescription);
            _sqliteDataProvider.AddParameter("@ProductGroup", product.Group);
            _sqliteDataProvider.AddParameter("@Brand", product.Brand);
            _sqliteDataProvider.AddParameter("@Model", product.Model);
            _sqliteDataProvider.AddParameter("@LastPrice", product.LastPrice);
            return _sqliteDataProvider.ExecuteNonQuery(query) > 0;
        }

        public bool UpdateProductLatestPrice(int productId, double price)
        {
            const string query = "Update Products " +
                                 "Set LastPrice = @LastPrice" +
                                 " Where Id = @Id";
            _sqliteDataProvider.AddParameter("@Id", productId);
            _sqliteDataProvider.AddParameter("@LastPrice", price);
            return _sqliteDataProvider.ExecuteNonQuery(query) > 0;
        }


        private Product CreateProduct(DataRow row)
        {
            return new Product
            {
                Id = Convert.ToInt32(row["Id"]),
                Code = row["Code"].ToString(),
                CrossReferenceCode = row["CrossReferenceCode"]?.ToString(),
                Brand = row["Brand"]?.ToString(),
                Description = row["Description"]?.ToString(),
                EnglishDescription = row["DescriptionEng"]?.ToString(),
                Group = row["ProductGroup"]?.ToString(),
                Model = row["Model"]?.ToString(),
                LastPrice = Convert.IsDBNull(row["LastPrice"]) || string.IsNullOrEmpty(row["LastPrice"].ToString()) ? 0 : Convert.ToDouble(row["LastPrice"])
            };
        }
    }
}
