using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AnyCode.Models.Service;
using DBlinq;
using AnyCode.Models;
using Common;

namespace AnyCode.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController()
            : base(new LinqToDB())
        {
        }

        public ActionResult Index()
        {
            var dc = new GodBuildDB();
            ViewData["menu"] = LoginUser.Sys_Role.Pid;
            ViewData["username"] = LoginUser.MyName;
            ViewData["isSystem"] = LoginUser.IsSystem;
            ViewData["userId"] = LoginUser.Id;
            ViewBag.HeadImgUrl = LoginUser.HeadImgUrl;
            ViewBag.HomeNewsList = HomeNewsList();
            WebInfo();
            return View();
        }

        [HttpPost]
        public JsonResult GetMenu(string pids)
        {
            var fid = LoginUser.Sys_Role.Fid;
            var sysMenuService = new SysMenuService(LoginUser);
            var pidLoadList = new List<string>();
            if (!LoginUser.IsSystem) 
            {
                pidLoadList = pids.Split(',').ToList();
            }
            else//超级管理员
            {
                pidLoadList = Db.Sys_Menu.Select(c => c.Id.ToString(CultureInfo.InvariantCulture)).ToList();
            }

            var menus = sysMenuService.GetMenu(pidLoadList, fid);
            return Json(menus);
        }

        public ActionResult Exit()
        {
            lock (MvcApplication.lockObject)
            {
                UserTicket.Users.RemoveAll(c => c.Id == UserTicket.Id);//移除该用户
            }
            AccountBLL.DelteCookie("AnyCode");

            return Json("1");
        }

        [HttpPost]
        public JsonResult ReBack()
        {
            if (Session["SystemUserID"] != null)
            {
                Session["userId"] = Session["SystemUserID"];
                return Json("1");
            }
            return Json("0");
        }




        /// <summary>
        /// 控制台上公告列表内容
        /// </summary>
        /// <returns></returns>
        public IList HomeNewsList()
        {
            var search = from s1 in Db.News_User_Company.Where(c => c.Code == LoginUser.Id)
                             orderby s1.IsRead ascending, s1.CreateTime descending
                             select new
                             {
                                 s1.Id,
                                 s1.News,
                                 s1.Title,
                                 s1.CreateTime,
                                 s1.IsRead
                             };
                return search.Take(7).ToList();

        }

    
        public JsonpResult TableDemo(DataGridParam param)
        {
            var data = from tt in Db.JB_IsaacTable
                       select tt;
           return new JsonpResult
            {
                Data=new 
                {
                    Total = data.Count(),
                    DataGridParam=param,
                    Rows=data.Skip((param.Page-1)*param.RP).Take(param.RP)
                },
                JsonRequestBehavior=JsonRequestBehavior.AllowGet
            };
        }

    }
}
