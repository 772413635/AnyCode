using System;
using System.Web.Mvc;
using Common;
using DBlinq;
using AnyCode.Models.Interfaces;
using AnyCode.Models.Service;

namespace AnyCode.Controllers
{
    public class ContentController : BaseController
    {
        private readonly IContentService _content;

        public ContentController()
        {
            _content = new ContentService(new LinqToDB(), LoginUser);
        }

        /// <summary>
        /// 文本页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Text()
        {
            return View();
        }

        /// <summary>
        /// 图像页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Image()
        {
            return View();
        }

        /// <summary>
        /// 视频页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Video()
        {
            return View();
        }

        public JsonResult TextList(DataGridParam param)
        {
            return Json(_content.TextList(param));

        }

        public JsonResult PwdList(DataGridParam param)
        {
            return Json(_content.PwdList(param));
        }

        public string ShowContent(int id)
        {
            return _content.ShowContent(id);
        }
         [OutputCache(VaryByParam = "*", Duration = 3600)]
        public JsonpResult JShowContent(DataGridParam param)
        {
            return new JsonpResult
            {
                Data = _content.ShowContent(param.RP),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// job-在线留言
        /// </summary>
        /// <param name="param">跨越参数</param>
        /// <returns></returns>
        public JsonpResult JJobMessage(DataGridParam param)
        {
            return new JsonpResult
            {
                Data = _content.JJobMessage(new JB_JobMessage
                {
                    Name = param.SortName,
                    Email=param.SortOrder,
                    Content=param.Query,
                    CreateTime=DateTime.Now
                }),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public string DeleteText(string id)
        {
            return _content.DeleteText<Sys_Text>(id);
        }

        public string DeletePwd(string id)
        {
            return _content.DeletePwd<Sys_Password>(id);
        }

        public ActionResult Text_Edit(int id)
        {
            return GetModelView<Sys_Text>("Id", id, "Text_Add");
        }

        public ActionResult Pwd_Edit(int id)
        {
            ViewBag.UserToken = LoginUser.UserToken;
            return GetModelView<Sys_Password>("Id", id, "Pwd_Add");
        }
        public ActionResult Text_Add()
        {
            ViewBag.UserToken = LoginUser.UserToken;
            return View();
        }

        public ActionResult Pwd()
        {
            ViewBag.UserToken = LoginUser.UserToken;
            return View();
        }

        /// <summary>
        /// 添加或更新text
        /// </summary>
        /// <param name="t">序列化过来的实体</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public string CreateText(Sys_Text t)
        {
            t.RoleId = LoginUser.RoleId;
            var updateModel = new
            {
                t.Title,
                t.Content
            };
            return _content.CreateOrUpdate("Id", t.Id, 0, t, updateModel);
        }

        [HttpPost]
        public string CreatePwd(Sys_Password t)
        {
            var updateModel = new
            {
                t.Name,
                UserName=MeDes.encMe(t.UserName,LoginUser.UserToken) ,
                Password=MeDes.encMe(t.Password,LoginUser.UserToken),
                t.Remark
            };
            return _content.CreateOrUpdate("Id", t.Id, 0, t, updateModel);
        }
    }
}