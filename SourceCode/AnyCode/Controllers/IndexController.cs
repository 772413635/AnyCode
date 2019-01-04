using DBlinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnyCode.Controllers
{
    public class IndexController:BaseController
    {
        public IndexController()
          : base(new LinqToDB())
        {
        }
    }
}