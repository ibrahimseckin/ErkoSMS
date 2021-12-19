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
    public class PackingDataService
    {
        private IDataProvider _sqliteDataProvider;
        public PackingDataService()
        {
            _sqliteDataProvider = new SqliteDataProvider();
        }

        public IList<Pallet> GetAllPallets()
        {
            const string query = "select * from pallets";
            var dataset = _sqliteDataProvider.ExecuteDataSet(query);
            IList<Pallet> pallets = new List<Pallet>();
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                pallets.Add(CreatePallet(row));
            }

            return pallets;
        }

        //public Product GetProductById(int productId)
        //{
        //    const string query = "Select * From products Where Id = @productid";
        //    _sqliteDataProvider.AddParameter("@productid", productId);
        //    DataRow row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
        //    return CreatePallet(row);
        //}

        //public Product GetProductByCode(string productCode)
        //{
        //    const string query = "Select * From products Where lower(code) = lower(@productCode)";
        //    _sqliteDataProvider.AddParameter("@productCode", productCode);
        //    DataRow row = _sqliteDataProvider.ExecuteDataRows(query).FirstOrDefault();
        //    return row != null ? CreatePallet(row) : null;
        //}


        //public IList<Product> GetProductByCodeWithWildCard(string productCode)
        //{
        //    const string query = "Select * From products Where code like @productCode";
        //    _sqliteDataProvider.AddParameter("@productCode", $"%{productCode}%");
        //    var rows = _sqliteDataProvider.ExecuteDataRows(query);
        //    IList<Product> products = new List<Product>();
        //    foreach (DataRow row in rows)
        //    {
        //        products.Add(CreatePallet(row));
        //    }

        //    return products;
        //}

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

        public bool CreatePallet(IPallet product)
        {
            const string query = "Insert into Pallets (Width, Height, Depth, Description, DescriptionEng, Weight, GrossWeight) values " +
                                "(@Width,@Height,@Depth,@Description,@DescriptionEng,@Weight,@GrossWeight);" +
                                "select last_insert_rowid();";
            _sqliteDataProvider.AddParameter("@Width", product.Width);
            _sqliteDataProvider.AddParameter("@Height", product.Height);
            _sqliteDataProvider.AddParameter("@Depth", product.Depth);
            _sqliteDataProvider.AddParameter("@Description", product.Description);
            _sqliteDataProvider.AddParameter("@DescriptionEng", product.EnglishDescription);
            _sqliteDataProvider.AddParameter("@Weight", product.Weight);
            _sqliteDataProvider.AddParameter("@GrossWeight", product.GrossWeight);
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


        private Pallet CreatePallet(DataRow row)
        {
            return new Pallet
            {
                Id = Convert.ToInt32(row["Id"]),
                Width = Convert.ToInt32(row["Weight"]),
                Height = Convert.ToInt32(row["Height"]),
                Depth = Convert.ToInt32(row["Depth"]),
                Description = row["Description"]?.ToString(),
                EnglishDescription = row["DescriptionEng"]?.ToString(),
                Weight = Convert.ToInt32(row["Weight"]),
                GrossWeight = Convert.ToInt32(row["GrossWeight"]),
            };
        }
    }
}
