using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            const string query = "Select * From products Where id = @productid";
            _sqliteDataProvider.AddParameter("@productid", productId);
            DataRow row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
            return CreateProduct(row);
        }



        private Product CreateProduct(DataRow row)
        {
            return new Product
            {
                Id = Convert.ToInt32(row["id"]),
                Code = row["Code"].ToString(),
                CrossReferenceCode = row["CrossReferenceCode"]?.ToString(),
                Brand = row["Brand"]?.ToString(),
                Description = row["Description"]?.ToString(),
                EnglishDescription = row["DescriptionEng"]?.ToString(),
                Group = row["Group"]?.ToString(),
                Model = row["Model"]?.ToString(),
                LastPrice = Convert.IsDBNull(row["LastPrice"]) ? null : (double?) row["LastPrice"]
            };
        }
    }
}
