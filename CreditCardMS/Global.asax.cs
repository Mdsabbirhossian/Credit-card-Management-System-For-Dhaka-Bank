using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CreditCardMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            //Login ar status ar jonno 
            Session["Active"] = "Not Active";

            //client side page redirection
            @Session["page"] = "../Home/Index";

            //Login ar por user info jonno
            Session["UserName"] = "";
            Session["UserType"] = "";
            Session["UserId"] = "";
            Session["CardType"] = "";
            Session["CardNumber"] = "";
            var date = DateTime.Now.Date;
            var date2 = date.ToString("yyyy-MM-dd");
            Session["date"] = date2;
        }


    }
}
