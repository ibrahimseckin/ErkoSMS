// SIMOCRANE RCMS, Copyright (C)Siemens AG, 2017.


using System;
using System.Drawing;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ErkoSMS
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString Template(this HtmlHelper html, string partialViewName)
        {
            return html.Raw(HttpUtility.JavaScriptStringEncode(html.Partial(partialViewName).ToString()));
        }

        public static IHtmlString Template(this HtmlHelper html, string partialViewName, object model)
        {
            return html.Template(partialViewName, model, new ViewDataDictionary());
        }

        public static IHtmlString Template(this HtmlHelper html, string partialViewName, object model, ViewDataDictionary viewData)
        {
            return html.Raw(HttpUtility.JavaScriptStringEncode(html.Partial(partialViewName, model, viewData).ToString()));
        }
    }
}
