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

        public bool CreatePackedProduct(PackedProduct packedProduct)
        {
            const string query = "Insert into Packing (SalesId, PalletId, ProductId, Quantity) values " +
                                 "(@SalesId, @PalletId, @ProductId, @Quantity);" +
                                 "select last_insert_rowid();";
            _sqliteDataProvider.AddParameter("@SalesId", packedProduct.OrderId);
            _sqliteDataProvider.AddParameter("@PalletId", packedProduct.PalletId);
            var productId = new ProductDataService().GetProductByCode(packedProduct.ProductCode).Id;
            _sqliteDataProvider.AddParameter("@ProductId", productId);
            _sqliteDataProvider.AddParameter("@Quantity", packedProduct.Quantity);
            var queryResult = _sqliteDataProvider.ExecuteScalar(query);
            
            return queryResult != null;
        }

        public IList<PackedProduct> GetPackedProductsByOrderId(int orderId)
        {
            const string query = "select * from Packing where SalesId = @SalesId;";
            _sqliteDataProvider.AddParameter("@SalesId", orderId);
            var dataset = _sqliteDataProvider.ExecuteDataSet(query);
            
            IList<PackedProduct> packedProducts = new List<PackedProduct>();
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                packedProducts.Add(CreatePackedProductObject(row));
            }

            return packedProducts;
        }

        public bool DeletePackedProductsByOrderId(int orderId)
        {
            const string query = "delete from Packing where SalesId = @SalesId;";
            _sqliteDataProvider.AddParameter("@SalesId", orderId);
            return _sqliteDataProvider.ExecuteNonQuery(query) > 0;
        }

        private PackedProduct CreatePackedProductObject(DataRow row)
        {
            var productId = Convert.ToInt32(row["ProductId"]);
            var productCode = new ProductDataService().GetProductById(productId).Code;
            return new PackedProduct
            {
                OrderId = Convert.ToInt32(row["SalesId"]),
                PalletId = Convert.ToInt32(row["PalletId"]),
                ProductCode = productCode,
                Quantity = Convert.ToInt32(row["Quantity"])
            };
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
