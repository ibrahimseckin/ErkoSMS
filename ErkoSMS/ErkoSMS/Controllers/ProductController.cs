using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ErkoSMS.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetProducts()
        {
            var products = new ProductDataService().GetAllProducts();

            var asd = products.Where(x => x.CrossReferenceCode.Length > 1000);
       
            return new JsonResult()
            {
                Data = products,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };


            //return Json(new { result }, JsonRequestBehavior.AllowGet);               
        }
    }
}