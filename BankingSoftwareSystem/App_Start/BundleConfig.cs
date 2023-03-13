using System.Web;
using System.Web.Optimization;

namespace BankingSoftwareSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/app").Include(
                 "~/Scripts/app/scripts/core.js",
                 "~/Scripts/app/scripts/script.js",
                 "~/Scripts/app/scripts/script.min.js",
                 "~/Scripts/app/scripts/jquery.steps.js",
                 "~/Scripts/app/scripts/steps-setting.js",
                 "~/Scripts/app/scripts/sweetalert2.all.js",
                 "~/Scripts/app/scripts/sweet-alert.init.js",
                 "~/Scripts/app/scripts/layout-settings.js",
                 "~/Scripts/app/scripts/apexcharts.min.js",
                 "~/Scripts/app/scripts/dashboard.js"
                 ));

            bundles.Add(new StyleBundle("~/Content/app").Include(
                "~/Content/app/styles/core.css",
                "~/Content/app/styles/icon-font.min.css",
                "~/Content/app/styles/jquery.steps.css",
                "~/Content/app/styles/style.css",
                "~/Content/app/styles/sweetalert2.css"
                ));
        }
    }
}
