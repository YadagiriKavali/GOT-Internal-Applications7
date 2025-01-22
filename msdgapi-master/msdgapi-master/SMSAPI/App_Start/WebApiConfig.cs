using System.Web.Http;

namespace SMSAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "billdetailservice",
                routeTemplate: "api/getbilldetail",
                defaults: new { controller = "Common", action = "GetBillDetail" }
            );

            config.Routes.MapHttpRoute(
                name: "service",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
