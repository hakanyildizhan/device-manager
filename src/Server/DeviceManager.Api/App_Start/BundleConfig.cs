// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.Web.Optimization;

namespace DeviceManager.Api
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/ClientLibraries/jquery/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-unobtrusive").Include(
                        "~/ClientLibraries/jquery-ajax-unobtrusive/jquery.unobtrusive-ajax.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/ClientLibraries/modernizr/modernizr.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/ClientLibraries/twitter-bootstrap/js/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-wizard").Include(
                        "~/ClientLibraries/twitter-bootstrap-wizard/jquery.bootstrap.wizard.min.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                        "~/ClientLibraries/twitter-bootstrap/css/bootstrap.min.css",
                        "~/Content/Styles/site.css",
                        "~/Content/Styles/loading-button.css"));

            bundles.Add(new StyleBundle("~/bundles/css-fontawesome").Include(
                        "~/ClientLibraries/font-awesome/css/all.min.css", new CssRewriteUrlWebRoot()));
        }
    }
}