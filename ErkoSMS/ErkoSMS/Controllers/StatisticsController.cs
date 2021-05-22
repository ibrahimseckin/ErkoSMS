using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ErkoSMS.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Statistics
        public ActionResult OrderStatistics()
        {
            return View();
        }


        public ActionResult CustomerHitList()
        {
            return View();
        }

        public ActionResult OrderStatusOverview()
        {
            return View();
        }

        public ActionResult ProductHitList()
        {
            return View();
        }

        public ActionResult SalesManStatistics()
        {
            return View();
        }


    }
}