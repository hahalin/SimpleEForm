using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace eform
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            //WebApiConfig.Register(GlobalConfiguration.Configuration);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    HttpRuntimeSection section = (HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime");
        //    int maxFileSize = section.MaxRequestLength * 1024;
        //    if (Request.ContentLength > maxFileSize)
        //    {
        //        try
        //        {
        //            Response.Redirect("~/Form/UploadError");
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        //}
    }
}
