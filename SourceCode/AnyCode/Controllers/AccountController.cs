using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using DBlinq;
using AnyCode.Models;

namespace AnyCode.Controllers
{
    public class AccountController : BaseController
    {

        public ActionResult Index()
        {
            WebInfo();
            return View();

        }

        [HttpPost]
        public string Login(Sys_User model)//登录系统
        {
            var user = AccountBLL.ValidateUser(model.Name, model.Password);
            if (user != null)//成功登录
            {
                AccountBLL.SaveCookie("AnyCode", user.Id.ToString(CultureInfo.InvariantCulture), DateTime.Now.AddDays(1), "/");//储存cookie
                if (!string.IsNullOrWhiteSpace(user.Theme))
                {
                    AccountBLL.SaveCookie("Theme", user.Theme.ToString(CultureInfo.InvariantCulture), DateTime.Now.AddDays(1), "/");//储存主题
                }
                lock (MvcApplication.lockObject)
                {
                    if (UserTicket.Users.All(c => c.Id != user.Id))
                    {
                        UserTicket.Users.Add(user);//用户成功登录后，就把这个用户添加到在线用户列表中
                    }

                }
                if (user.IsSystem == false)
                {
                    var log = new Sys_PerformLog
                    {
                        Ip = HttpContext.Request.UserHostAddress,
                        Controller = "Account",
                        Action = "Login",
                        CreateTime = DateTime.Now,
                        UserId = LoginUser.Id,
                        Params = ""
                    };
                    Db.InsertOnSubmit(log);
                    Db.SubmitChanges();
                }


                return "1";
            }
            return "账号或密码错误";

        }
    }
}
