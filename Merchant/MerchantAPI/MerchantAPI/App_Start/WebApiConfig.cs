using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MerchantAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Конфигурация и службы веб-API

            // Маршруты веб-API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "SingleCurrencyApi",
                routeTemplate: "paynet/api/v2/{controller}/{endpointId}" /*,
                defaults: new { endpointId = RouteParameter.Optional }*/
            );

            config.Routes.MapHttpRoute(
                name: "MultiCurrencyApi",
                routeTemplate: "paynet/api/v2/{controller}/group/{endpointGroupId}" /*,
                defaults: new {endpointGroupId = RouteParameter.Optional}*/
            );

            config.Routes.MapHttpRoute(
                name: "MultiCurrencyApiResult",
                routeTemplate: "paynet/api/v2/{controller}/group/{endpointGroupId}/{action}" /*,
                defaults: new {endpointGroupId = RouteParameter.Optional}*/
            );

        }
    }
}
