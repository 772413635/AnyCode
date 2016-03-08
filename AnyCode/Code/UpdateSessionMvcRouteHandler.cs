using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AnyCode
{
    public class UpdateSessionMvcRouteHandler : MvcRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new UpdateSessionMvcHandler(requestContext);
        }
    }
}