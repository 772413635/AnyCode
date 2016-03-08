using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Common;
using DBlinq;
using EfSearchModel;
using EfSearchModel.Model;
using AnyCode.Controllers;
using AnyCode.Models;
using MongoDB.Driver;
namespace AnyCode
{
    public abstract class BaseController : Controller
    {
        protected readonly LinqToDB Db;

        protected BaseController()//默认构造函数
        {
            Db = new LinqToDB();
        }

        protected BaseController(LinqToDB db)
        {
            Db = db;
        }

        /// <summary>
        /// 显现试图基础方法
        /// </summary>
        /// <param name="id">视图名</param>
        /// <returns></returns>
        public ActionResult GetView(string id)
        {
            return View(id);
        }

        public Sys_User GetCreatePerson()
        {
            var user = UserTicket.Users.SingleOrDefault(c => c.Id == UserTicket.Id);
            return user;
        }

        public Sys_User LoginUser
        {
            get { return GetCreatePerson(); }
        }

        /// <summary>
        /// action执行前执行此方法
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controlName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionDescriptor.ActionName;
            if (controlName.ToLower() == "account" || (controlName.ToLower() == "webchat" && actionName.ToLower() == "index"))//如果是登录则跳过安全认证
            {
                if (UserTicket.Users.Any(c => c.Id == UserTicket.Id))//如果用户上次为安全退出，并且认证cookie未丢失，则直接跳转至主页面
                {
                    HttpContext.Response.Redirect("~/Home/Index");
                }
                else
                {
                    return;
                }

            }
            //处理跨域安全认证问题
            var cross = CrossDomain(filterContext, actionName);

            if (cross.Flag) //返回类型不是jsonpResult，参数类型不是DataGridParam，参数对象中没有UserId属性
            {
                if (LoginUser == null) //如果未登录
                {

                    filterContext.Result = Redirect("/account/index");
                    return;

                }
                if (LoginUser.IsSystem == false) //除管理员外其他用户所有操作都得记录数据库
                {
                    string ip = filterContext.HttpContext.Request.UserHostAddress;
                    var sb = new StringBuilder();
                    foreach (var item in filterContext.ActionParameters.Keys)
                    {
                        string key = item;
                        Object obj = filterContext.ActionParameters[item];
                        if (obj != null)
                        {
                            Type t = obj.GetType();
                            if (t.FullName == "System.String" || t.FullName == "System.Int32")
                            {
                                sb.Append(key + "=" + obj + "/");

                            }
                            else
                            {
                                PropertyInfo[] properties = t.GetProperties();
                                foreach (PropertyInfo t1 in properties)
                                {
                                    string objKey = t1.Name;
                                    string value = Convert.ToString(t1.GetValue(obj, null));
                                    sb.Append(objKey);
                                    sb.Append("=");
                                    sb.Append(value);
                                    sb.Append("/");
                                }
                            }
                        }

                    }
                    var log = new Sys_PerformLog
                    {
                        Ip = ip,
                        Controller = controlName,
                        Action = actionName,
                        CreateTime = DateTime.Now,
                        UserId = LoginUser.Id,
                        UserName = LoginUser.Name,
                        Params = sb.ToString()
                    };
                    var collection = MongoDbHelper.DataBase.GetCollection("Sys_PerformLog");
                    collection.Insert<Sys_PerformLog>(log);
                }
            }
            else
            {
                if (cross.User == null)
                {
                    if (!filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonpResult()
                        {
                            Data = "权限不足",
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    else
                    {
                        filterContext.Result = Redirect("/account/index");
                    }
                }

            }


        }

        /// <summary>
        /// 处理跨越问题
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        [OutputCache(VaryByParam = "*", Duration = 3600)]
        private dynamic CrossDomain(ActionExecutingContext filterContext, string actionName)
        {
            var methods = filterContext.Controller.GetType().GetMethods();//根据方法名获取方法
            foreach (var mothod in methods.Where(c => c.Name == actionName))
            {
                var returnType = mothod.ReturnType;
                if (returnType == typeof(JsonpResult))//判断返回类型是否是JsonpResult
                {
                    foreach (var item in filterContext.ActionParameters.Keys)
                    {
                        var obj = filterContext.ActionParameters[item];
                        if (obj.GetType() == typeof(DataGridParam))//判断参数类型是否是DataGridParam
                        {
                            var userIdValue = obj.GetType().GetProperty("UserId").GetValue(obj, null);
                            if (userIdValue != null)//判断DataGridParam对象中是否有UserId属性
                            {
                                var user = UserTicket.Users.SingleOrDefault(c => c.Id == userIdValue.GetInt());//到在线用户列表中读取用户
                                Sys_User resUser;
                                if (user != null) //用户在线
                                {
                                    resUser = user;
                                }
                                else //用户不在线
                                {
                                    var userModel = Db.Sys_User.SingleOrDefault(c => c.Id == userIdValue.GetInt());
                                    resUser = userModel;

                                }
                                return new
                                {
                                    Flag = false,
                                    User = resUser
                                };//找到UserId后就不再遍历
                            }
                        }
                    }
                }
            }
            return new
            {
                Flag = true
            };
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var error = new StringBuilder();
            error.AppendLine("Error Url :: " + (Request.Url == null ? "null" : Request.Url.ToString()));
            error.AppendLine("------ Exception ------");
            error.AppendLine(GetErrorCode(filterContext.Exception));
            Logger.Log(error.ToString());
            Server.ClearError();
        }

        public static string GetErrorCode(Exception ex)
        {
            if (ex == null)
                return "";
            var error = new StringBuilder();
            error.AppendLine("Class Name :: " +
                             ((ex.TargetSite == null || ex.TargetSite.DeclaringType == null)
                                  ? "null"
                                  : ex.TargetSite.DeclaringType.FullName));
            error.AppendLine("Method Name :: " + (ex.TargetSite == null ? "null" : ex.TargetSite.Name));
            error.AppendLine("Message :: " + (ex.Message));
            error.AppendLine("StackTrace :: " + (ex.StackTrace ?? "null"));
            error.AppendLine("Source :: " + (ex.Source ?? "null"));
            error.AppendLine("InnerException :: " + (ex.InnerException == null
                                                        ? "null"
                                                        : ex.InnerException.ToString()));
            return error.ToString();
        }

        /// <summary>
        /// 返回model到页面
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="pkName">主键名</param>
        /// <param name="pkValue">主键值</param>
        /// <param name="page">页面名称</param>
        /// <returns></returns>
        public ActionResult GetModelView<T>(string pkName, dynamic pkValue, string page) where T : class, new()
        {
            var dc = new GodBuildDB();
            var qM = new QueryModel();
            var item = new ConditionItem
            {
                Field = pkName,
                Value = pkValue,
                Method = QueryMethod.Equal
            };
            qM.Items.Add(item);
            var query = dc.GetTable<T>().Where(qM);
            return View(page, query.Single());
        }

        //返回视图基方法
        protected void BaseView(string url)
        {
            var menu = Db.Sys_Menu.SingleOrDefault(c => c.Url == url);
            if (menu != null)
            {
                ViewBag.Title = menu.Name;
            }
            else
            {
                ViewBag.Title = "ViewBag.Title";
            }
        }

        /// <summary>
        /// 生成网站的一些信息
        /// </summary>
        public void WebInfo()
        {
            var company = Db.Sys_Company.Single();
            ViewBag.WebTitle = company.Title;
            ViewBag.Footer = company.Footer;
        }

    }


}

