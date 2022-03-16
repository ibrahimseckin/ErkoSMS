using ErkoSMS.Enums;
using ErkoSMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ErkoSMS.DataAccess.Model;

namespace ErkoSMS.Controllers
{

    public class HomeController : Controller
    {

        LeftMenuViewModel AdministratorNavigations = new LeftMenuViewModel
        {
            Navigations = new List<LeftMenu>
            {
                    new LeftMenu(new LeftMenuItem("Kullanıcı Yönetimi", "UserAdministration", "Index"),null),
                    new LeftMenu(new LeftMenuItem("Ürünler", "Product", "Index"),null),
                    new LeftMenu(new LeftMenuItem("Müşteriler", "Customer", "Index"),null),
                    new LeftMenu(new LeftMenuItem("Satış Yönetimi", "", ""),
                        new List<LeftMenuItem>{new LeftMenuItem("Satış Listele/Güncelle","Order","ListOrder"),
                        new LeftMenuItem("Yeni Satış","Order","CreateOrder")
                        }),
                    new LeftMenu(new LeftMenuItem("Stok Listesi", "Stock", "Index"),null),
                    new LeftMenu(new LeftMenuItem("Tedarikçi Yönetimi", "", ""),
                        new List<LeftMenuItem>{new LeftMenuItem("Tedarikçi Listele/Güncelle","Supplier","ListSuppliers"),
                            new LeftMenuItem("Yeni Tedarikçi Girişi","Supplier","CreateSupplier")
                        }),
                    new LeftMenu(new LeftMenuItem("Satın Alma Yönetimi", "", ""),
                        new List<LeftMenuItem>{new LeftMenuItem("Satın Alma Listele/Güncelle","Purchase","ListPurchase"),
                            new LeftMenuItem("Yeni Satın Alma","Purchase","CreatePurchase")
                        }),
                    new LeftMenu(new LeftMenuItem("İthalatçı Yönetimi", "", ""),
                        new List<LeftMenuItem>{new LeftMenuItem("İthalatçı Listele/Güncelle","Exporter","ListExporters"),
                            new LeftMenuItem("Yeni İthalatçı Girişi","Exporter","CreateExporter")
                        }),

                     new LeftMenu(new LeftMenuItem("İstatistik", "", ""),
                        new List<LeftMenuItem>
                        {
                            new LeftMenuItem("Satış Durum Genel Bakış","Statistics","OrderStatusOverview"),
                            new LeftMenuItem("Satış Personeli İstatistikleri","Statistics","SalesManStatistics"),
                            new LeftMenuItem("Ürün İstatistikleri","Statistics","ProductHitList"),
                            new LeftMenuItem("Müşteri İstatistikleri","Statistics","CustomerHitList")
                        }),
            }
        };

        private LeftMenuViewModel SalesNavigations = new LeftMenuViewModel
        {
            Navigations = new List<LeftMenu>
            {
                new LeftMenu(new LeftMenuItem("Satış Yönetimi", "", ""),
                    new List<LeftMenuItem>{new LeftMenuItem("Satış Listele/Güncelle","Order","ListOrder"),
                        new LeftMenuItem("Yeni Satış","Order","CreateOrder")
                    }),
                 new LeftMenu(new LeftMenuItem("Ürünler", "Product", "Index"),null)
            }
        };

        private LeftMenuViewModel WarehouseNavigations = new LeftMenuViewModel
        {
            Navigations = new List<LeftMenu>
            {
                new LeftMenu(new LeftMenuItem("Paletler", "Pallet", "Index"),null),
                new LeftMenu(new LeftMenuItem("Paketleme Bekleyen Siparişler", "Packing", "ListUnpackedOrders"),null),
                new LeftMenu(new LeftMenuItem("Paketlenmiş Siparişler", "Packing", "ListPackedOrders"),null)
            }
        };

        LeftMenuViewModel AnynomousNavigations = new LeftMenuViewModel
        {
            Navigations = new List<LeftMenu> {
                new LeftMenu(new LeftMenuItem("Home", "Home", "Index"),null),
                new LeftMenu(new LeftMenuItem("About", "Home", "About"),null),
                new LeftMenu(new LeftMenuItem("Contact", "Home", "Contact"),null),
            }
        };

        LeftMenuViewModel PurchaserNavigations = new LeftMenuViewModel
        {
            Navigations = new List<LeftMenu>
            {
                new LeftMenu(new LeftMenuItem("Ürünler", "Product", "Index"),null),
                new LeftMenu(new LeftMenuItem("Satış Yönetimi", "", ""),
                    new List<LeftMenuItem>{new LeftMenuItem("Satış Listele/Güncelle","Order","ListOrder"),
                        new LeftMenuItem("Yeni Satış","Order","CreateOrder")
                    }),
                new LeftMenu(new LeftMenuItem("Stok Listesi", "Stock", "Index"),null),
                new LeftMenu(new LeftMenuItem("Tedarikçi Yönetimi", "", ""),
                    new List<LeftMenuItem>{new LeftMenuItem("Tedarikçi Listele/Güncelle","Supplier","ListSuppliers"),
                        new LeftMenuItem("Yeni Tedarikçi Girişi","Supplier","CreateSupplier")
                    }),
                new LeftMenu(new LeftMenuItem("Satın Alma Yönetimi", "", ""),
                    new List<LeftMenuItem>{new LeftMenuItem("Satın Alma Listele/Güncelle","Purchase","ListPurchase"),
                        new LeftMenuItem("Yeni Satın Alma","Purchase","CreatePurchase")
                    }),
            }
        };

        [Authorize]
        public ActionResult Index()
        {
            var stockHistoryHelper = new StockHistoryHelper();
            stockHistoryHelper.UpdateStockHistory();
            ViewBag.Role = User.IsInRole(UserTypes.Administrator.Name) ? UserTypes.Administrator.Name : "";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult LeftMenu(string selectedMenu)
        {
            var userNavigations = GetUserNavigation();
            ViewBag.SelectedMenu = selectedMenu;
            return PartialView("_LeftMenu", userNavigations);
        }

        private LeftMenuViewModel GetUserNavigation()
        {
            if (User.IsInRole(UserTypes.Administrator.Name))
            {
                return AdministratorNavigations;
            }
            if (User.IsInRole(UserTypes.Accountant.Name))
            {
                return AnynomousNavigations;
            }
            if (User.IsInRole(UserTypes.Purchaser.Name))
            {
                return PurchaserNavigations;
            }
            if (User.IsInRole(UserTypes.SalesMan.Name))
            {
                return SalesNavigations;
            }
            if (User.IsInRole(UserTypes.WareHouseMan.Name))
            {
                return WarehouseNavigations;
            }

            return AnynomousNavigations;

        }




    }
}