using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Common;
using DBlinq;

namespace AnyCode.Controllers
{
    public class SystemController : BaseController
    {
        private readonly ISysSystem _system;
        public SystemController()
        {
            _system =new SysSystem(new LinqToDB(), LoginUser);
        }

        public ActionResult UserManager()
        {
            ViewData["companyId"] = LoginUser.Cid;
            ViewData["companyName"] = LoginUser.Sys_Company.CompanyName;
            return View();
        }

        /// <summary>
        /// 角色管理界面
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleManager()
        {
            ViewData["companyId"] = LoginUser.Cid;
            return View();
        }

        /// <summary>
        /// 新建用户页面
        /// </summary>
        /// <returns></returns>
        public ActionResult User_Add()
        {
            var query = Db.Sys_UserStatus.ToList();
            ViewData["UserStatus"] = new SelectList(query, "Id", "Name");
            ViewData["companyId"] = LoginUser.Cid;
            return View("User_Add");
        }
        public ActionResult Role_Add()
        {
            return View();
        }

        public ActionResult LoginUserManager()
        {
            return View();
        }

        /// <summary>
        /// 编辑用户页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult User_Edit(string id)
        {
            var query = Db.Sys_UserStatus.ToList();
            ViewData["UserStatus"] = new SelectList(query, "Id", "Name");
            ViewData["companyId"] = LoginUser.Cid;
            var sss = Db.Sys_User.SingleOrDefault(c => c.Id == Int32.Parse(id));
            if (sss != null)
            {
                ViewData["Id"] = sss.Id;
                return View("User_Add", sss);
            }
            return null;
        }

        /// <summary>
        /// 平台日志页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PerformLog()
        {
            return View();
        }

        /// <summary>
        /// 错误日志页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ErrorLog()
        {
            return View();
        }

        /// <summary>
        /// 菜单管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult MenuManager()
        {
            return View();
        }

        public ActionResult GetRoleModelView(string id)
        {
            return GetModelView<Sys_Role>("Id",id, "Role_Add");
        }

        public ActionResult NewsDelivery(string id)
        {
            ViewData["People"] = LoginUser.Id;
            return View();
        }

        public ActionResult MenuManagerEdit(string id)
        {
            var data = from tt in Db.Sys_Menu
                       where tt.IsColumn
                       select new
                       {
                           tt.Id,
                           tt.Name
                       };
            ViewBag.dropType = new SelectList(data, "Id", "Name");
            ViewData["isEdit"] = 1;
            ViewBag.MyTitle = "编辑菜单";


            var query = Db.Sys_Menu.Single(c => c.Id == Int32.Parse(id));//数据
            var functions = (from tt in Db.Sys_Function
                             where tt.Status
                             orderby tt.Sort descending
                             select new FunctionS
                             {
                                 Id = tt.Id,
                                 Name = tt.Name,
                                 IsSelect = false
                             }).ToList();
            if (!string.IsNullOrWhiteSpace(query.Function))
            {
                var selectIds = query.Function.Split(',');
                foreach (string dd in selectIds)
                {
                    var fq = functions.Where(c => c.Id == Int32.Parse(dd));
                    if (fq.Any())
                    {
                        fq.Single().IsSelect = true;
                    }
                }
            }
            ViewBag.Functions = functions;//功能模块列表


            return View("MenuManager_Add", query);
        }

        //新增菜单
        public ActionResult MenuManagerAdd()
        {
            var data = from tt in Db.Sys_Menu
                       where tt.IsColumn
                       select new
                       {
                           tt.Id,
                           tt.Name
                       };
            ViewBag.dropType = new SelectList(data, "Id", "Name");
            ViewData["isEdit"] = 0;
            ViewBag.MyTitle = "新增菜单";


            var functions = (from tt in Db.Sys_Function
                             where tt.Status
                             orderby tt.Sort descending
                             select new FunctionS
                             {
                                 Id = tt.Id,
                                 Name = tt.Name,
                                 IsSelect = false
                             }).ToList();
            ViewBag.Functions = functions;//功能模块列表

            return View("MenuManager_Add");
        }

        public ActionResult SystemManager()
        {
            BaseView(HttpContext.Request.Path.TrimStart('/'));
            return View();
        }

        public ActionResult OnlineUser(DataGridParam param)
        {
            return _system.OnlineUser(param);
        }

        public string SingOutUser(string id)
        {
            return _system.SingOutUser(id);
        }

        public JsonResult GetMenu(DataGridParam param)
        {
            param.Query += "[Equal]IsColumn&False^";
            return _system.GetData<Sys_Menu>(param);
        }

        [HttpPost]
        public string DeleteMenu(string id)
        {
            return _system.SystemDeleteData<Sys_Menu>(id);
        }

        [HttpPost]
        public string MenuManagerAdd(Sys_Menu m)
        {
            m.CompanyWeb = true;
            m.SystemWeb = true;
            m.State = true;
            var updateModel = new
            {
                m.Name,
                m.ParentId,
                m.Sort,
                m.Url,
                m.Controller,
                m.Action,
                m.Iconic,
                m.Remark,
                m.Function
            };
            return _system.CreateOrUpdate("Id", m.Id, 0, m, updateModel);
        }


        public ActionResult EditAllMenu(string id)
        {
            int[] ids = id.Split(',').GetIntArray();
            var dc = new GodBuildDB();
            ViewBag.Type = new SelectList(dc.Sys_Menu.Where(c => c.IsColumn).ToList(), "Id", "Name");
            var queryName = dc.Sys_Menu.Where(c => ids.Contains(c.Id)).Select(c => c.Name).ToArray();
            var sname = new StringBuilder("");
            foreach (var s in queryName)
            {
                sname.Append(s + ",");
            }
            ViewBag.SName = sname.ToString().TrimEnd(',');
            ViewBag.Id = id;
            return View("EditAllMenu");
        }

        [HttpPost]
        public string EditAllMenu(Sys_Menu t)
        {
            var dc = new GodBuildDB();
            var ps = (from tt in t.GetType().GetProperties().Where(c => c.GetValue(t, null) != null && c.Name != "Id" && c.Name != "Controller" && c.Name != "Action")
                      select new
                      {
                          tt.Name,
                          Value = tt.GetValue(t, null)
                      }).ToList();
            var query = from kk in dc.Sys_Menu
                        where ps.Single(c => c.Name == "Name").Value.GetString().Split(',').GetIntArray().Contains(kk.Id)
                        select kk;
            foreach (var q in query)
            {
                foreach (var p in ps.Where(c => c.Name != "Name"))
                {
                    q.GetType().GetProperty(p.Name).SetValue(q, p.Value, null);
                }
            }
            dc.SubmitChanges();
            return Suggestion.UpdateSucceed;
        }


        [HttpPost]
        public JsonResult GetColumn(string id)
        {
            return _system.GetColumn(id);
        }

        public JsonResult GetUserList(string id)
        {
            return _system.GetUserList(id);
        }

        [HttpPost]
        public string DeleteUser(string id)
        {
            return _system.SystemDeleteData<Sys_User>(id);
        }

        [HttpPost]
        public string DeleteRole(string id)
        {
            var list = new List<Sys_User>();
            var query = Db.GetTable<Sys_Role>().Where(c => id.Split(',').GetIntArray().Contains(c.Id)).ToList();
            foreach (var q in query)
            {
                list.AddRange(q.Sys_User.ToList());
            }

            Db.GetTable<Sys_User>().DeleteAllOnSubmit(list);
            Db.GetTable<Sys_Role>().DeleteAllOnSubmit(query);
            Db.SubmitChanges();
            return Suggestion.DeleteSucceed;
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="t">用户对象</param>
        /// <returns></returns>
        [HttpPost]
        public string CreateUser(Sys_User t)
        {
            if (t.Id == 0)
            {
                t.Password = MD5.Encrypt(t.Password, 32);
                t.Cid = LoginUser.Cid;
            }

            return _system.CreateOrUpdate("Id",t.Id,0,t,new
            {
                t.Name,
                t.MyName,
                t.MobilePhoneNumber,
                t.PhoneNumber,
                t.Email,
                t.Status,
                t.RoleId
            });
        }


        /// <summary>
        /// 添加和编辑角色
        /// </summary>
        /// <param name="t">角色model</param>
        /// <returns></returns>
        [HttpPost]
        public string CreateRole(Sys_Role t)
        {
            if (t.Id == 0)
            {
                t.Cid = LoginUser.Cid;
                t.IsDelete = false;
            }
            return _system.CreateOrUpdate("Id", t.Id, 0, t, new
            {
                t.RoleName,
                t.IsSystem,
                t.Pid,
                t.Fid
            });
        }

        [HttpPost]
        public string ChangePwd(ChangePwdSeatModel user)
        {
            if (user != null)
            {
                if (MD5.Encrypt(user.Oldpassword, 32) != LoginUser.Password)
                {
                    return "请输入正确的原始密码！";
                }
                if (user.Password != user.ConfirmPassword)
                {
                    return "2次输入的密码不一致！";
                }
                var updateuser = Db.Sys_User.Single(c => c.Id == LoginUser.Id);
                updateuser.Password = MD5.Encrypt(user.Password, 32);
                var result = _system.EditUserPwd(LoginUser.Id.ToString(CultureInfo.InvariantCulture), updateuser);
                if (result == Suggestion.UpdateSucceed)
                    LoginUser.Password = MD5.Encrypt(user.Password, 32);
                return result;
            }
            return Suggestion.UpdateFail;
        }

        public JsonResult GetUserTable(string id, DataGridParam param)
        {
            return Json(_system.GetUserTable(id, param));
        }

        public JsonResult GetRoleTable(string id, DataGridParam param)
        {
            return Json(_system.GetRoleTable(id, param));
        }

        [HttpPost]
        public JsonResult GetRoleList(string id)
        {
            return _system.GetRoleList(id);
        }

        //根据角色id获取用户权限
        public JsonResult GetCompetenceByRoleId(string id)
        {
            return _system.GetCompetenceByRoleId(id);
        }

        //根据用户id获取用户权限
        public JsonResult GetCompetencebyUserId(string id)
        {
            return _system.GetCompetencebyUserId(id);
        }

        [HttpPost]
        public JsonResult GetSystemSelectTree(string id)
        {
            return _system.GetSystemSelectTree(id);
        }


        [HttpPost]
        public string ReplacePwd(string id)
        {
            string rt = _system.ChangeUserPassword(id);
            return rt;
        }

        /// <summary>
        /// 系统日志数据
        /// </summary>
        /// <param name="param">查询对象</param>
        /// <returns></returns>
        public JsonResult GetPerformLogList(DataGridParam param)
        {
            return _system.GetPerformLogList(param);
        }

        [HttpPost]
        public JsonResult GetErrorFilesList()
        {
            return _system.GetErrorFilesList();
        }

        [HttpPost]
        public string ReadFile(string path)
        {
            return _system.ReadFile(path);
        }


        [HttpPost]
        public JsonResult IconList()
        {
            return Json(_system.IconList());
        }
    }
}
