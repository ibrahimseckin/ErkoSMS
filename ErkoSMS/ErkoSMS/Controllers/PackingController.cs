using ErkoSMS.Enums;
using ErkoSMS.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.Model;
using ErkoSMS.Objects;
using Microsoft.Office.Interop.Excel;

namespace ErkoSMS.Controllers
{

    public class PackingController : Controller
    {
        [HttpGet]
        public ActionResult ListUnpackedOrders()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ListPackedOrders()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePallet(PalletViewModel pallet)
        {
            var packingDataService = new PackingDataService();
            var result = packingDataService.CreatePallet(pallet);
            if (result)
            {
                return Json(new AjaxResult("Sandık başarıyla kaydedildi."));
            }
            return Json(new AjaxResult("Sandık kaydedilemedi!"));
        }

        [HttpGet]
        public ActionResult GetReadyForPackingOrders()
        {
            var allOrders = new SalesDataService().GetSalesByState(SalesState.WaitForPacking);
            var orderedOrders = allOrders.OrderBy(x => x.InvoiceDate);
            var allOrdersForView = orderedOrders.Select(x => new OrderViewModel(x));
            return new JsonResult()
            {
                Data = allOrdersForView,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public ActionResult GetPackedOrders()
        {
            var allOrders = new List<Sales>();
            var packingReadyOrders = new SalesDataService().GetSalesByState(SalesState.PackingIsReady);
            var packingReadyWaitingPaymentOrders = new SalesDataService().GetSalesByState(SalesState.PackingIsReadyAndWaitForPayment);

            allOrders.AddRange(packingReadyOrders);
            allOrders.AddRange(packingReadyWaitingPaymentOrders);

            var orderedOrders = allOrders.OrderBy(x => x.InvoiceDate);
            var allOrdersForView = orderedOrders.Select(x => new OrderViewModel(x));
            return new JsonResult()
            {
                Data = allOrdersForView,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public ActionResult PackOrder(int orderId)
        {
            var salesDataService = new SalesDataService();
            var order = salesDataService.GetSalesById(orderId);

            ViewBag.Order = new OrderViewModel(order);

            return PartialView();
        }

        [HttpGet]
        public ActionResult EditOrderPacking(int orderId)
        {
            var salesDataService = new SalesDataService();
            var packingDataService = new PackingDataService();

            var order = salesDataService.GetSalesById(orderId);
            var packedProducts = packingDataService.GetPackedProductsByOrderId(orderId);

            var packing = new PackingViewModel
            {
                OrderId = orderId,
                Pallets = new List<PackingViewModel.PackingPallet>()
            };

            foreach (var packedProduct in packedProducts)
            {
                var packedPrdct = new PackingViewModel.PackingPallet.PackedProduct
                {
                    ProductCode = packedProduct.ProductCode,
                    Quantity = packedProduct.Quantity
                };

                var palletOfProduct = packing.Pallets.FirstOrDefault(x => x.PalletId == packedProduct.PalletId);

                if (palletOfProduct != null)
                {
                    palletOfProduct.Products.Add(packedPrdct);
                }
                else
                {
                    palletOfProduct = new PackingViewModel.PackingPallet
                    {
                        PalletId = packedProduct.PalletId,
                        Products = new List<PackingViewModel.PackingPallet.PackedProduct>()
                    };
                    palletOfProduct.Products.Add(packedPrdct);
                    packing.Pallets.Add(palletOfProduct);
                }
            }

            ViewBag.Order = new OrderViewModel(order);
            ViewBag.Packing = packing;

            return PartialView();
        }

        [HttpGet]
        public ActionResult PackProduct()
        {
            var packingDataService = new PackingDataService();
            var allPallets = packingDataService.GetAllPallets();


            ViewBag.Pallets = allPallets.Select(x => new PalletViewModel(x)).ToList();

            return PartialView();
        }

        [HttpPost]
        public ActionResult CreatePacking(PackingViewModel packingDetail)
        {
            var packingDataService = new PackingDataService();
            foreach (var packingDetailPallet in packingDetail.Pallets)
            {
                foreach (var packedProduct in packingDetailPallet.Products)
                {
                    packingDataService.CreatePackedProduct(new PackedProduct
                    {
                        OrderId = packingDetail.OrderId,
                        PalletId = packingDetailPallet.PalletId,
                        ProductCode = packedProduct.ProductCode,
                        Quantity = packedProduct.Quantity
                    });
                }
            }

            var salesDataService = new SalesDataService();
            salesDataService.UpdateOrderState(packingDetail.OrderId, SalesState.PackingIsReady);

            return Json(new AjaxResult(true));
        }

        [HttpPost]
        public ActionResult UpdatePacking(PackingViewModel packingDetail)
        {
            var packingDataService = new PackingDataService();
            packingDataService.DeletePackedProductsByOrderId(packingDetail.OrderId);
            foreach (var packingDetailPallet in packingDetail.Pallets)
            {
                foreach (var packedProduct in packingDetailPallet.Products)
                {
                    packingDataService.CreatePackedProduct(new PackedProduct
                    {
                        OrderId = packingDetail.OrderId,
                        PalletId = packingDetailPallet.PalletId,
                        ProductCode = packedProduct.ProductCode,
                        Quantity = packedProduct.Quantity
                    });
                }
            }

            return Json(new AjaxResult(true));
        }

        [HttpGet]
        public ActionResult GetPackedProductsExportInfo(int orderId)
        {
            var packingExportInfo = new PackingDataService().GetPackingExportInfo(orderId);

            var groups = packingExportInfo.GroupBy(x => x.PalletId).ToList();

            foreach (var packing in groups)
            {
                foreach (var productExport in packing.Skip(1))
                {
                    productExport.PalletId = string.Empty;
                    productExport.Dimensions = string.Empty;
                    productExport.NetKG = string.Empty;
                    productExport.GrossKG = string.Empty;
                }
            }

            var filteredEmptyRows = packingExportInfo.Where(x => !string.IsNullOrEmpty(x.PalletId)).ToList();
            var lastRow = new PackedProductExport
            {
                PalletId = "TOPLAM",
                Quantity = filteredEmptyRows.Sum(x => Convert.ToInt32(x.Quantity)).ToString(),
                NetKG = filteredEmptyRows.Sum(x => Convert.ToDouble(x.NetKG)).ToString(),
                GrossKG = filteredEmptyRows.Sum(x => Convert.ToDouble(x.GrossKG)).ToString(),
                ProductCode = string.Empty,
                Description = string.Empty,
                Dimensions = string.Empty
            };

            packingExportInfo.Add(lastRow);

            return new JsonResult()
            {
                Data = packingExportInfo,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}