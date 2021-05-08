﻿using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.Model;
using ErkoSMS.ViewModels;
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
            return new JsonResult()
            {
                Data = products,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };              
        }


        [HttpGet]
        public ActionResult GetProductByCode(string productCode)
        {
            var product = new ProductDataService().GetProductByCode(productCode);
            return new JsonResult()
            {
                Data = product != null ? new List<Product> { product } : new List<Product>(),
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };            
        }

        [HttpGet]
        public ActionResult DeleteProductByCode(string productCode)
        {
            var result = new ProductDataService().DeleteProductByCode(productCode);
            return new JsonResult()
            {
                Data = result,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        public ActionResult CreateProduct()
        {
            return PartialView();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public  ActionResult CreateProduct(ProductViewModel product)
        {
            var productDataService = new ProductDataService();
            var result = productDataService.CreateProduct(product);
            return new JsonResult()
            {
                Data = result,
                ContentType = "application/json"
            };
        }


    }
}