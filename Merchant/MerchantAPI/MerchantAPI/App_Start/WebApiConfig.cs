using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace MerchantAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Конфигурация и службы веб-API

            // Маршруты веб-API
            config.MapHttpAttributeRoutes();

            // Hyphenated urls for controller name in WebApi
            config.Services.Replace(typeof(IHttpControllerSelector),
                new ApiControllerSelector(config));

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

    public class ApiControllerSelector : DefaultHttpControllerSelector
    {
        public ApiControllerSelector(HttpConfiguration configuration) : base(configuration) { }

        public override string GetControllerName(HttpRequestMessage request)
        {
            // add logic to remove hyphen from controller name lookup of the controller
            return base.GetControllerName(request).Replace("-", string.Empty);
        }
    }
}
