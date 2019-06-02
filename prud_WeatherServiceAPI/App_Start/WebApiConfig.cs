using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Unity;
using prud_WeatherServiceAPI.Models;
using prud_WeatherService_BL;
using prud_WeatherService_BL.Entities;
using Unity.Injection;
using Unity.Lifetime;

namespace prud_WeatherServiceAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new UnityContainer();
            //Process objprocess = new Process();
            //InjectionConstructor injectionConstructor = new InjectionConstructor(objprocess);
            container.RegisterType<IProcessFile, ProcessTextFile>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);
            

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
