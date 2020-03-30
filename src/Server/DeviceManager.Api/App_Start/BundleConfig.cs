using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace DeviceManager.Api
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/ClientLibraries/jquery/jquery.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/ClientLibraries/modernizr/modernizr.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/ClientLibraries/twitter-bootstrap/js/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-wizard").Include(
                        "~/ClientLibraries/twitter-bootstrap-wizard/jquery.bootstrap.wizard.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/ClientLibraries/twitter-bootstrap/css/bootstrap.css",
                        "~/Content/Styles/site.css",
                        "~/Content/Styles/loading-button.css"));
        }
    }
}