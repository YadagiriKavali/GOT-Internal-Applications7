using System.Web;
using System.Web.Optimization;

namespace AadharAdmin
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                       "~/Scripts/jquery.validate*"));

            ///////////////////////////////////////////////////////////////////////////////////
            ////********************Custom Bundling Configuration**********Strated*************
            ///////////////////////////////////////////////////////////////////////////////////

            //////********************CSS Bundling****************************************///
            bundles.Add(new StyleBundle("~/assets/css/CustomMaterializeCss").Include(
                      "~/assets/css/materialize.min.css"));

            bundles.Add(new StyleBundle("~/assets/css/CustomCss").Include(
                      "~/assets/css/custom.css"));

            bundles.Add(new StyleBundle("~/Scripts/an-js/CustomMatCss").Include(
                      "~/Scripts/an-js/angular-material.min.css"));

            bundles.Add(new StyleBundle("~/Plugins/toaster/CustomToasterCss").Include(
                      "~/Plugins/toaster/bootoast.min.css"));

            bundles.Add(new StyleBundle("~/Plugins/JQdatatables/CustomJQDTCss").Include(
                      "~/Plugins/JQdatatables/datatables.min.css"));

            bundles.Add(new StyleBundle("~/Plugins/daterangepicker/CustomDtRangePickerCss").Include(
                      "~/Plugins/daterangepicker/daterangepicker.css"));

            bundles.Add(new StyleBundle("~/Plugins/Bootstrap/CustomBootstrapCss").Include(
                      "~/Plugins/Bootstrap/bootstrap.min.css",
                      "~/Plugins/select2/select2.css"));
            //////********************JS Bundling****************************************///
            bundles.Add(new ScriptBundle("~/Scripts/CustomJqueryJs").Include(
                       "~/Scripts/jqueryV321.min.js"));


            bundles.Add(new ScriptBundle("~/assets/js/CustomMaterializeJs").Include(
                       "~/assets/js/materialize.min.js"));


            bundles.Add(new ScriptBundle("~/Scripts/an-js/CustomAngularJs").Include(
                       "~/Scripts/an-js/angular.min.js",
                       "~/Scripts/an-js/angular-animate.min.js",
                       "~/Scripts/an-js/angular-aria.min.js",
                       "~/Scripts/an-js/angular-messages.min.js"));

            bundles.Add(new ScriptBundle("~/Scripts/an-js/CustomMatJs").Include(
                       "~/Scripts/an-js/angular-material.min.js"));

            bundles.Add(new ScriptBundle("~/Plugins/toaster/CustomToasterJs").Include(
                       "~/Plugins/toaster/bootoast.min.js"));


            bundles.Add(new ScriptBundle("~/Plugins/JQdatatables/CustomJQDTJs").Include(
                        "~/Plugins/JQdatatables/datatables.min.js"));

            bundles.Add(new ScriptBundle("~/Plugins/daterangepicker/CustomDtRangePickerJs").Include(
                       "~/Plugins/daterangepicker/moment.min.js",
                       "~/Plugins/daterangepicker/daterangepicker.js"));

            bundles.Add(new ScriptBundle("~/Plugins/Bootstrap/CustomBootstrapJs").Include(
                       "~/Plugins/Bootstrap/popper.min.js",
                       "~/Plugins/Bootstrap/bootstrap.min.js",
                       "~/Plugins/select2/select2.js"));
            /////////////////////////////////////////////////////////////////////////////////
            ////********************Custom Bundling Configuration********Ended***************
            /////////////////////////////////////////////////////////////////////////////////

            BundleTable.EnableOptimizations = true;
        }
    }
}
