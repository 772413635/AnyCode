using System;
using System.Web.Routing;

namespace AnyCode
{
    public static class UpdateSessionRouteCollectionExtensions
    {
        public static void MapUpdateSessionRoute(this RouteCollection routes, String name, String url)
        {
            MapUpdateSessionRoute(routes, name, url, null);
        }
        public static Route MapUpdateSessionRoute(this RouteCollection routes, String name, String url, object defaults)
        {
            var route = new Route(url, new UpdateSessionMvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults)
            };
            routes.Add(name, route);
            return route;
        }
    }
}