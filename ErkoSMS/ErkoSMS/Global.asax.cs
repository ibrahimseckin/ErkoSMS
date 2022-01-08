using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NLog;

namespace ErkoSMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            var httpContext = HttpContext.Current;
            var exception = Server.GetLastError();
            Logger.Error(exception, $"Application wide unhandled exception. URL: '{httpContext?.Request.Url}'");
        }
    }
}
