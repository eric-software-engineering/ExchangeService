using System.Net.Http.Headers;
using System.Web.Http;
using ExchangeService.Models;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using SimpleInjector.Integration.WebApi;
using ExchangeService.Interfaces;
using ExchangeService.Models.Database;
using ExchangeService.Models.RedisCache;

namespace ExchangeService
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // Web API configuration and services
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

      // Registering types
      container.Register<IHTTPClientAdapter, HTTPClientAdapter>(Lifestyle.Scoped);
      container.Register<IRepository<IHTTPClientAdapter>, FixerClientAdapter>(Lifestyle.Scoped);
      container.Register<IRepository<DataModel>, DatabaseAccess>(Lifestyle.Scoped);
      container.Register<IRepository<AzureRedisControllerCache>, CacheAccess>(Lifestyle.Scoped);

      // This is an extension method from the integration package.
      container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

      container.Verify();

      GlobalConfiguration.Configuration.DependencyResolver =
        new SimpleInjectorWebApiDependencyResolver(container);

      // Web API routes
      config.MapHttpAttributeRoutes();

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );

      config.Formatters.JsonFormatter.SupportedMediaTypes
        .Add(new MediaTypeHeaderValue("text/html"));
    }
  }
}
