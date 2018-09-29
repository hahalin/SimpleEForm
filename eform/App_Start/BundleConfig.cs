using System.Web;
using System.Web.Optimization;

namespace eform
{
    public class BundleConfig
    {
        //http://www.howtosolutions.net/2017/05/visual-studio-asp-net-mvc-project-installing-adminlte-control-panel/
        // 如需統合的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/toastr.min.css",
                      //"~/Content/ionicons.min.css",
                      "~/Content/font-awesome.min.css",
                      "~/admin-lte/css/AdminLTE.css",
                      "~/admin-lte/css/skins/skin-blue.css",
                      "~/admin-lte/css/skins/skin-purple-light.min.css",
                      "~/admin-lte/css/skins/skin-purple.min.css",
                      "~/Content/site.css",
                      "~/Content/bootstrap-select.min.css",
                      "~/Content/select2.min.css",
                      "~/Content/themes/base/jquery-ui.css",
                      "~/Scripts/tabulator-master/dist/css/tabulator.min.css",
                      "~/Scripts/tabulator-master/dist/css/tabulator_site.min.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js",
                        "~/Scripts/jquery-ui-1.12.1.min.js",
                        "~/Scripts/jquery.blockUI.js",
                        "~/Scripts/jquery.validate.min.js"
            ));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好可進行生產時，請使用 https://modernizr.com 的建置工具，只挑選您需要的測試。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));


            bundles.Add(new ScriptBundle("~/start").Include(
                "~/admin-lte/js/adminlte.js",
                "~/Scripts/toastr.min.js",
                "~/Scripts/bootstrap-select.min.js",
                "~/Scripts/select2.full.js",
                "~/Scripts/tabulator-master/dist/js/tabulator.min.js",
                "~/Scripts/TabulatorEditors.js",
                "~/Scripts/DateUtil.js"
                //"~/admin-lte/plugins/fastclick/fastclick.js"
             ));

        }
    }
}
