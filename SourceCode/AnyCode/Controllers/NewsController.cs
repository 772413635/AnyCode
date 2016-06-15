using System;
using System.Linq;
using System.Web.Mvc;
using Common;
using DBlinq;

namespace AnyCode.Controllers
{
    public class NewsController : BaseController
    {
        private readonly ISysSystem _system;

        public NewsController()
        {
            _system = new SysSystem(new LinqToDB(), LoginUser);
        }

        /// <summary>
        /// 显示某一个公告
        /// </summary>
        /// <param name="id">公告id</param>
        /// <returns></returns>
        public ActionResult ShowNews(string id)
        {
            News_User_Company result;

            var search = from s1 in Db.News_User_Company.Where(o => o.Id == Int32.Parse(id) && o.Code == LoginUser.Id)
                         select s1;

            result = search.ToList().Single();
            if (result.IsRead == false)
            {
                Sys_WebNewsCode updateData = Db.GetTable<Sys_WebNewsCode>().Single(c => c.NewId == Int32.Parse(id) && c.Code == LoginUser.Id);
                updateData.IsRead = true;
                Db.SubmitChanges();
            }
            return View("ShowNews", result);
        }


        public ActionResult NewsList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetData(DataGridParam param)
        {
            return _system.LoadNewList(param);
        }

        /// <summary>
        /// 控制台上公告列表内容
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult HomeNewsList()
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
            return Json(search.Take(7).ToList());
        }

        /// <summary>
        /// 处理字符串
        /// </summary>
        /// <param name="oldString">原字符串</param>
        /// <param name="maxNumber">最大个数</param>
        /// <returns>新字符串</returns>
        public string Elipsis(string oldString, int maxNumber)
        {
            int lineNumber = oldString.Length;
            if (maxNumber > lineNumber)
                return oldString;
            string newString = oldString.Substring(0, maxNumber) + "......";
            return newString;
        }

    }
}
