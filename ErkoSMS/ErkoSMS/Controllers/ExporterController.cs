using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ErkoSMS.DataAccess;
using ErkoSMS.DataAccess.DataService;
using ErkoSMS.DataAccess.Model;
using ErkoSMS.Objects;
using ErkoSMS.ViewModels;

namespace ErkoSMS.Controllers
{
    public class ExporterController : Controller
    {
        public ActionResult Index()
        {
            return ListExporters();
        }


        public ActionResult ListExporters()
        {

            return View();
        }

        [HttpGet]
        public ActionResult GetAllExporters()
        {
            var allExporters = new ExporterDataService().GetAllExporters();

            return new JsonResult()
            {
                Data = allExporters,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult CreateExporter()
        {
            return View(new ExporterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateExporter(ExporterViewModel exporter)
        {
            new ExporterDataService().CreateExporter(exporter);
            return Json(new AjaxResult(true));

        }

        public ActionResult UpdateExporter(int id)
        {
            var exporterDataService = new ExporterDataService();
            var exporter = exporterDataService.GetExporter(id);
            return PartialView(new ExporterViewModel(exporter));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateExporter(ExporterViewModel exorter)
        {
            var exporterDataService = new ExporterDataService();
            var result = exporterDataService.UpdateExporter(exorter);

            return Json(result ? new AjaxResult(true) : new AjaxResult(false));
        }

        [HttpGet]
        public ActionResult DeleteExporter(int exporterId)
        {
            var result = new ExporterDataService().DeleteExporter(exporterId);
            return new JsonResult()
            {
                Data = result,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }
    }
}