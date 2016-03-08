using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AnyCode
{
    public class UpdateSessionMvcHandler : MvcHandler
    {
        public UpdateSessionMvcHandler(RequestContext requestContext)
            : base(requestContext)
        {
        }
        protected override void ProcessRequest(HttpContext httpContext)
        {
            base.ProcessRequest(httpContext);

            //string LastUpdateSessionSeverTime = DateTime.Now.ToString();
            //HttpContext.Current.Response.Cookies["LastUpdateSessionTime"].Value = LastUpdateSessionSeverTime;
            string s = MilliTimeStamp(DateTime.Now).ToString();
            HttpContext.Current.Response.Cookies["LastUpdateSessionTime"].Value = s;
        }
        public long MilliTimeStamp(DateTime TheDate)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = TheDate.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return (long)ts.TotalMilliseconds;
        }
    }
}