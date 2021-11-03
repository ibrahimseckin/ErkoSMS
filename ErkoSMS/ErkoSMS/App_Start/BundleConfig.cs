﻿using System.Web;
using System.Web.Optimization;

namespace ErkoSMS
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/layout").Include(
                     "~/Content/vex/js/vex.combined.js",
                      "~/CustomContent/Scripts/defaultComponentConfiguration.js",
                      "~/CustomContent/Scripts/base.js",
                      "~/Scripts/moment-with-locales.js",
                      "~/Scripts/select2.min.js",
                      "~/Scripts/Chart.js",
                     "~/Scripts/jquery-ui-1.12.1.min.js",
                     "~/Scripts/jspdf.umd.min.js",
                     "~/Scripts/jspdf-autotable.js",
                     "~/Scripts/DataTables/media/js/jquery.dataTables.min.js",
                      "~/Scripts/DataTables/extensions/Buttons/js/dataTables.buttons.min.js",
                      "~/Scripts/DataTables/extensions/Buttons/js/buttons.html5.min.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/w3.css",
                      "~/Content/vex/css/vex.css",
                      "~/CustomContent/Styles/vex-theme-metro.css",
                      "~/CustomContent/Styles/Layout.css",
                      "~/Content/css/select2.min.css",
                      "~/Content/themes/base/jquery-ui.min.css",
                      "~/Content/DataTables/media/css/jquery.dataTables.min.css"));

            bundles.Add(new StyleBundle("~/Content/login").Include(
                     "~/CustomContent/Styles/login.css",
                      "~/Content/bootstrap.css"));
        }
    }
}
