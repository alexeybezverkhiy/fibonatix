using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using MerchantAPI.App_Start;

namespace MerchantAPI
{
    public static class WebApiConfig
    {
        public static readonly ISettings Settings;
        public static readonly SettingsFactory SettingsFactory;

        static WebApiConfig()
        {
//            Settings = new ProductionSettings();
            Settings = new StageSettings();
//            Settings = new TestSettings();
            SettingsFactory = new SettingsFactory(Settings);
        }

        public static void Register(HttpConfiguration config)
        {
            // Configuration and Services of Web API
            
            // Routing of Web API
            config.MapHttpAttributeRoutes();

            // Hyphenated urls for controller name in WebApi
            config.Services.Replace(typeof(IHttpControllerSelector),
                new ApiControllerSelector(config));

            config.Routes.MapHttpRoute(
                name: "A-SingleCurrencyApi",
                routeTemplate: "paynet/api/v2/{controller}/{endpointId}" /*,
                defaults: new { endpointId = RouteParameter.Optional }*/
            );

            // Performing 'successurl' & 'failureurl' as {action}
            config.Routes.MapHttpRoute(
                name: "A-SingleCurrencyApiCallback",
                routeTemplate: "paynet/api/v2/{controller}/{endpointId}/{action}" /*,
                defaults: new { endpointId = RouteParameter.Optional }*/
            );

            config.Routes.MapHttpRoute(
                name: "A-NotificationPostbackOrManagement",
                routeTemplate: "paynet/api/v2/{controller}/{action}"
            );

//            config.Routes.MapHttpRoute(
//                name: "B-MultiCurrencyApi",
//                routeTemplate: "paynet/api/v2/{controller}/group/{endpointGroupId}" /*,
//                defaults: new {endpointGroupId = RouteParameter.Optional}*/
//            );

//            // Performing 'successurl' & 'failureurl' as {action}
//            config.Routes.MapHttpRoute(
//                name: "B-MultiCurrencyApiCallback",
//                routeTemplate: "paynet/api/v2/{controller}/group/{endpointGroupId}/{action}" /*,
//                defaults: new {endpointGroupId = RouteParameter.Optional}*/
//            );
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
