using System;
using System.Web.Mvc;
using DBlinq;
using AnyCode.Models.Interfaces;
using AnyCode.Models.Service;
using Common;
using System.Linq;
using System.Net;
using System.Text;
using AnyCode.Models;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;
using Newtonsoft.Json;

namespace AnyCode.Controllers
{
    public class WebChatController : BaseController
    {
        private readonly IWebChatService _service;
        private readonly LinqToDB _db;
        public WebChatController()
        {
            _db = new LinqToDB();
            _service = new WebChatService(_db, LoginUser);
        }


        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr, string token)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, token))
            {
                return Content(echostr);//返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, token) + "。如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel, string token)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, token))
            {
                return Content("参数错误！");
            }
            var webChat = _db.Sys_WebChat_Config.Single(c => c.Token == token);

            postModel.Token = token;
            postModel.EncodingAESKey = webChat.EncodingAESKey;
            postModel.AppId = webChat.AppId;

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            const int maxRecordCount = 10;

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new CustomMessageHandler(Request.InputStream, postModel, maxRecordCount, _db, webChat)
            {
                OmitRepeatedMessage = false
            };

            /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
         * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/


            //执行微信处理过程
            messageHandler.Execute();
            return new WeixinResult(messageHandler);
        }


        [HttpGet]
        public void WxLogin(string code)
        {

            var accessToken = GetHttpData<AccessToken>("https://api.weixin.qq.com/sns/oauth2/access_token",
                        "?appid=wxc4a45051a808cbf1" +
                        "&secret=b2416ec8ce8317cbf9211fd8ce681934" +
                        "&code=" + code +
                        "&grant_type=authorization_code"
                    );

            var userinfo = GetHttpData<UserInfo>("https://api.weixin.qq.com/sns/userinfo",
                        "?access_token=" + accessToken.access_token +
                        "&openid=" + accessToken.openid
                    );


            var searchUser = Db.Sys_User.SingleOrDefault(c => c.openid == userinfo.openid);
            if (searchUser != null)//微信号有绑定的用户
            {
                if (UserTicket.Users.All(c => c.Id != searchUser.Id))//用户没有登录
                {
                    UserTicket.Id = searchUser.Id;
                    UserTicket.Theme = searchUser.Theme;
                    lock (MvcApplication.lockObject)
                    {
                        UserTicket.Users.Add(searchUser);
                    }
                }
            }
            else//微信号没有有绑定的用户
            {
                //新增用户
                var com = Db.Sys_Company.FirstOrDefault();
                var user = new Sys_User
                {
                    Cid = com == null ? "" : com.Id,
                    Name = userinfo.openid,
                    MyName = userinfo.nickname,
                    Password = MD5.Encrypt("123456", 32),
                    Address = string.Format("{0} {1} {2}", userinfo.country, userinfo.province, userinfo.city),
                    Remark = "微信登录用户",
                    Status = 1,
                    CreatePerson = 2,
                    IsSystem = true,
                    RoleId = 1,
                    Theme = "deepblue",
                    UserToken = Guid.NewGuid().ToString("D"),
                    openid = userinfo.openid
                };
                Db.Sys_User.InsertOnSubmit(user);
                Db.SubmitChanges();
                //向用户池中添加用户
                UserTicket.Id = searchUser.Id;
                UserTicket.Theme = searchUser.Theme;
                lock (MvcApplication.lockObject)
                {
                    UserTicket.Users.Add(searchUser);
                }
            }
            HttpContext.Response.Redirect("~/Home/Index");
        }

        public T GetHttpData<T>(string url, string data)
        {
            var client = new WebClient();
            byte[] bResponse = client.DownloadData(url + data);
            string returnText = Encoding.UTF8.GetString(bResponse);
            return JsonConvert.DeserializeObject<T>(returnText);
        }

        public class AccessToken
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string openid { get; set; }
            public string scope { get; set; }
            public string unionid { get; set; }

        }

        public class UserInfo
        {
            public string openid { get; set; }
            public string nickname { get; set; }
            public int sex { get; set; }
            public string language { get; set; }
            public string city { get; set; }
            public string province { get; set; }
            public string country { get; set; }

            public string headimgurl { get; set; }
            public string[] privilege { get; set; }
            public string unionid { get; set; }

        }




        public ActionResult WxAccount()
        {
            return View();
        }

        public ActionResult WxAccount_Add()
        {
            return View();
        }

        public ActionResult WxAccount_Edit(string id)
        {
            var model = _db.Sys_WebChat_Config.Single(c => c.Id == Int32.Parse(id));
            return View("WxAccount_Add", model);
        }

        public ActionResult WxMenuManger()
        {
            var query = from tt in _db.Sys_WebChat_MenuType
                        select new SelectListItem
                        {
                            Value = tt.TypeId.ToString(),
                            Text = tt.Name
                        };
            ViewBag.menuType = query;
            return View();
        }

        public ActionResult KeyWord()
        {
            var msgType = _db.Sys_WebChat_MsgType.Where(c => c.IsReply);
            ViewBag.MsgType = new SelectList(msgType, "MsgType", "Name");
            return View();
        }

        //保存或更新数据
        [HttpPost]
        public string CreateWxAccount(Sys_WebChat_Config t)
        {
            var updateEntity = new
            {
                t.UserName,
                t.PassWord,
                t.AppName,
                t.AppId,
                t.AppSecret,
                t.Token,
                t.EncodingAESKey
            };
            return _service.CreateOrUpdate("Id", t.Id, 0, t, updateEntity);
        }

        // 删除微信账号
        public string DeleteAccount(string id)
        {
            return _service.DeleteAccount<Sys_WebChat_Config>(id);
        }

        [HttpPost]
        public JsonResult WxCfgList(DataGridParam param)
        {
            return Json(_service.WxCfgList(param));
        }

        [HttpPost]
        public JsonResult AccountList()
        {
            var param = new DataGridParam();
            param.Page = 1;
            param.RP = Int32.MaxValue;
            param.Query = "AppName[Contains]&" + Request.Form["q"];
            return Json(_service.WxCfgList(param).rows);
        }

        //验证token值的合法性
        [HttpPost]
        public JsonResult SingleToken(AjaxValidate ajaxValidate)
        {
            return Json(_service.SingleToken(ajaxValidate));
        }

        public JsonResult MenuList(int id)
        {
            var res = _service.Menu(id);
            return Json(res);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string DelteMenu(string menuid)
        {
            var menu = _db.GetTable<Sys_WebChat_Menu>().SingleOrDefault(c => c.Id == Int32.Parse(menuid));
            if (menu != null && menu.Fid == null) //主菜单
            {
                var childMenus = _db.GetTable<Sys_WebChat_Menu>().Where(c => c.Fid == menu.Id); //查询子菜单的菜单
                if (childMenus.Any())
                {
                    _db.GetTable<Sys_WebChat_Menu>().DeleteAllOnSubmit(childMenus);
                }
                _db.GetTable<Sys_WebChat_Menu>().DeleteOnSubmit(menu);
                _db.SubmitChanges();
                return Suggestion.DeleteSucceed;
            }
            else
            {
                return _service.DelteMenu(menuid);
            }



        }

        /// <summary>
        /// 重命名微信菜单
        /// </summary>
        /// <param name="menuid">菜单id</param>
        /// <param name="newName">新的名称</param>
        /// <returns></returns>
        [HttpPost]
        public string ReName(int menuid, string newName)
        {
            return _service.ReName(menuid, newName);
        }

        //添加一级菜单
        public string AddFartherMenu(string name, int cfgId)
        {
            return _service.AddFartherMenu(name, cfgId);
        }
        //添加二级菜单
        public string AddChildrenMenu(string name, int cfgId, int fid)
        {
            var updateFatherMenu = _service.UpdateFartherMenu(fid);
            if (updateFatherMenu == Suggestion.UpdateSucceed)
            {
                return _service.AddChildrenMenu(name, cfgId, fid);
            }
            return Suggestion.InsertFail;

        }

        //设置二级菜单
        public string SetChildrenBtn(int id, int type, string val)
        {
            return _service.SetChildrenBtn(id, type, val);
        }

        //生成微信菜单
        [HttpPost]
        public JsonResult CreateMenu(int cfgid, string appId)
        {
            var wxres = _service.CreateMenu(cfgid, appId);
            return Json(wxres);
        }

        public JsonResult AutoReplyList(DataGridParam param)
        {
            return Json(_service.AutoReplyList(param));
        }
    }
}