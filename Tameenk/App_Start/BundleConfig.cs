using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Tameenk.App_Start
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/resources/js/jquery.js",
                        "~/resources/js/slick.min.js",
                        "~/resources/js/jquery.select2.js",
                        "~/resources/js/pikaday.js",
                        "~/resources/js/jquery.calendars.min.js",
                        "~/resources/js/jquery.calendars.plus.min.js",
                        "~/resources/js/jquery.plugin.min.js",
                        "~/resources/js/jquery.calendars.picker.min.js",
                        "~/resources/js/jquery.calendars.islamic.min.js",
                        "~/resources/js/jquery.calendars.islamic-ar.js",
                        "~/resources/js/jquery.calendars-ar.js",
                        "~/resources/js/jquery.calendars.picker-ar.js",
                        "~/resources/js/forms.js",
                        "~/resources/js/app.js",
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js"));

            ////bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            ////            "~/Scripts/jquery.validate*"));

            ////// Use the development version of Modernizr to develop with and learn from. Then, when you're
            ////// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            ////bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            ////            "~/Scripts/modernizr-*"));

            ////bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            ////          "~/Scripts/bootstrap.js",
            ////          "~/Scripts/respond.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                            "~/resources/css/toastr.css",
                            "~/resources/css/redmond.calendars.picker.css",
                            "~/resources/css/select.theme.css",
                            "~/resources/css/tooltip.css",
                            "~/resources/css/modal.css",
                            "~/resources/css/layout.css",
                            "~/resources/css/override.css"));
        BundleTable.EnableOptimizations = true;
        }
    }
}
