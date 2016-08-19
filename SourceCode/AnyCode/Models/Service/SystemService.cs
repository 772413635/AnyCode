using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web.Mvc;
using AnyCode.Models;
using Common;
using DBlinq;
using EfSearchModel;
using EfSearchModel.Model;
using MongoDB.Driver;

namespace AnyCode
{
    public class SysSystem : BaseService, ISysSystem
    {
        public SysSystem(LinqToDB db, Sys_User loginUser)
            : base(db, loginUser)
        {
        }

        public JsonResult GetData<T>(DataGridParam param) where T : class,new()
        {
            return DataSearch<T>(param);
        }

        public JsonResult GetPerformLogList(DataGridParam param)
        {
            var qm = param.Query.GetModel();
            var tquery = from tt in _db.Sys_PerformLog.OrderBy(param.SortName+" "+ param.SortOrder)
                join cc in _db.Sys_User on tt.UserId equals cc.Id
                select new
                {
                    tt.Id,
                    UserName = cc.Name,
                    NickName=cc.MyName,
                    tt.Ip,
                    tt.Controller,
                    tt.Action,
                    tt.Params,
                    tt.CreateTime
                };
            var query = tquery.Where(qm);
            var jq = new JqueryGridObject
            {
                DataGridParam = param,
                total = query.Count(),
                Query = query.Skip((param.Page-1)*param.RP).Take(param.RP)
            };
            return new JsonResult { Data = jq };

        }
        public string SystemDeleteData<T>(string id) where T : class, new()
        {
            return DeleteData<T>(id);
        }
        public string SystemDeleteData_string<T>(string id) where T : class, new()
        {
            return DeleteData_string<T>(id);
        }

        public JsonResult GetColumn(string id)
        {
            var query = from tt in _db.Sys_Menu
                        where tt.IsColumn && tt.Name.Contains(id ?? "")
                        orderby tt.CreateTime ascending
                        select new
                        {
                            id = tt.Id,
                            text = tt.Name,
                            iconCls = tt.Iconic,
                            sort = tt.Sort
                        };
            return new JsonResult { Data = query.ToList() };
        }

        public JsonResult GetUserList(string id)
        {
            if (string.Equals(id, "all"))
            {
                var query = from tt in _db.Sys_User
                            where tt.Id != LoginUser.Id
                            select new
                                {
                                    tt.Id,
                                    tt.MyName,
                                    tt.Name
                                };
                return new JsonResult { Data = query.ToList() };

            }
            return null;
        }

        public string EditUserPwd<T>(string id, T t) where T : class, new()
        {
            return UpdateData(id, t);
        }

        public JsonResult GetRoleList(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var data = from role in _db.Sys_Role
                           where role.Cid == id && role.IsDelete == false
                           select new
                           {
                               id = role.Id,
                               text = role.RoleName,
                               role.IsSystem
                           };

                var result = new JsonResult { Data = data.ToList() };
                return result;
            }
            return null;
        }

        public JsonResult GetSystemSelectTree(string id)
        {
            var lastTrees = new List<SystemTree>();
            var extraParent = new SystemTree { id = "-1", text = "所拥有的权限" };
            var treeHelper = new PrimaryTreeNodeCollection();
            var targetMenuList = new List<SystemTree>();
            IEnumerable<Sys_Menu> trees;
            var pid = (from tt in _db.Sys_Role
                       where tt.Id == Int32.Parse(id)
                       select new
                       {
                           tt.Pid
                       }).SingleOrDefault();
            int[] pids = { };
            if (pid != null && !string.IsNullOrWhiteSpace(pid.Pid))
            {
                pids = pid.Pid.Split(',').GetIntArray();
            }
            trees = from o in _db.Sys_Menu
                    where o.CompanyWeb && o.State == true && pids.Contains(o.Id)
                    orderby o.Sort
                    select o;//获得所有菜单

            var userSonPreview = new List<string>();

            treeHelper.BindData(trees, ref targetMenuList, userSonPreview, "");

            extraParent.children.AddRange(targetMenuList);//添加子树

            lastTrees.Add(extraParent);//添加树
            return new JsonResult { Data = lastTrees };
        }

        //根据角色id获取相应的权限
        public JsonResult GetCompetenceByRoleId(string id)
        {
            var lastList = new List<SystemTree>();
            var treeHelper = new PrimaryTreeNodeCollection();
            var parentTree = new SystemTree();
            parentTree.id = "-1";
            parentTree.text = "选中全部";
            var targetMenuList = new List<SystemTree>();
            IEnumerable<Sys_Menu> trees;


            string cid = LoginUser.Cid;
            List<int> allParentPreview = (from o in _db.Sys_Menu
                                          where o.CompanyWeb && o.State == true && o.ParentId == null
                                          select o.Id).ToList();
            trees = from o in _db.Sys_Menu
                    where o.CompanyWeb && o.State == true
                    orderby o.Sort
                    select o;
            var role = (from u in _db.Sys_Role
                        where u.Id == Convert.ToInt32(id) && u.Cid == cid
                        select u).FirstOrDefault();

            int[] rolePreview;//用户权限
            List<string> userSonPreview = new List<string>();
            if (role != null)
            {
                if (!string.IsNullOrEmpty(role.Pid))
                {
                    rolePreview = role.Pid.Split(',').GetIntArray();
                    foreach (var onepreview in rolePreview)
                    {
                        if (!allParentPreview.Contains(onepreview))
                        {
                            userSonPreview.Add(onepreview.ToString(CultureInfo.InvariantCulture));
                        }
                    }
                    treeHelper.BindData(trees, ref targetMenuList, userSonPreview, role.Fid);
                }
                else
                {
                    treeHelper.Bind(trees, ref targetMenuList, "");
                }
            }
            parentTree.children.AddRange(targetMenuList);



            lastList.Add(parentTree);
            return new JsonResult { Data = lastList };
        }

        //根据用户id获取用户的权限
        public JsonResult GetCompetencebyUserId(string id)
        {
            var lastTrees = new List<SystemTree>();
            var extraParent = new SystemTree { id = "-1", text = "所拥有的权限" };
            var treeHelper = new PrimaryTreeNodeCollection();
            var targetMenuList = new List<SystemTree>();
            IEnumerable<Sys_Menu> trees;
            var user =
                _db.Sys_User.FirstOrDefault(o => o.Id == Int32.Parse(id));
            if (user != null)
            {
                string[] userPreview = user.Sys_Role.Pid.Split(',');//获取用户权限
                trees = from o in _db.Sys_Menu
                        where o.CompanyWeb && o.State == true && userPreview.Contains(o.Id.ToString())
                        orderby o.Sort
                        select o;//获得所有菜单
                var userSonPreview = new List<string>();

                userSonPreview.AddRange(userPreview);
                treeHelper.BindData(trees, ref targetMenuList, userSonPreview, "");
                extraParent.children.AddRange(targetMenuList);
                lastTrees.Add(extraParent);
            }
            else
            {
                trees = from o in _db.Sys_Menu
                        where o.CompanyWeb && o.State == true
                        orderby o.Sort
                        select o;//获得所有菜单

                var userSonPreview = new List<string>();

                treeHelper.BindData(trees, ref targetMenuList, userSonPreview, "");

                extraParent.children.AddRange(targetMenuList);//添加子树

                lastTrees.Add(extraParent);//添加树

            }
            return new JsonResult { Data = lastTrees };
        }

        public JqueryGridObject GetUserTable(string id, DataGridParam param)
        {
            var qm = param.Query.GetModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                var cn = new ConditionItem
                {
                    Field = "Cid",
                    Method = QueryMethod.Equal,
                    Value = id
                };
                qm.Items.Add(cn);
            }
            var query = from tt in _db.Sys_User.Where(qm)
                        orderby tt.CreateTime descending
                        where !tt.IsSystem
                        select new
                            {
                                tt.Id,
                                tt.Name,
                                tt.Sys_Role.RoleName,
                                tt.MyName,
                                statusName = tt.Sys_UserStatus.Name,
                                tt.MobilePhoneNumber,
                                tt.PhoneNumber,
                                tt.IsSystem,
                                tt.CreateTime,
                                tt.UpdateTime
                            };
            return new JqueryGridObject
                {
                    DataGridParam = param,
                    total = query.Count(),
                    Query = query.Skip((param.Page - 1) * param.RP).Take(param.RP)
                };


        }

        public JqueryGridObject GetRoleTable(string id, DataGridParam param)
        {
            var qm = param.Query.GetModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                var cn = new ConditionItem
                {
                    Field = "Cid",
                    Method = QueryMethod.Equal,
                    Value = id
                };
                qm.Items.Add(cn);
            }
            var query = from tt in _db.Sys_Role.Where(qm)
                        select new
                            {
                                tt.Id,
                                tt.RoleName,
                                tt.IsSystem,
                                tt.Pid
                            };
            return new JqueryGridObject
            {
                DataGridParam = param,
                total = query.Count(),
                Query = query.Skip((param.Page - 1) * param.RP).Take(param.RP)
            };
        }

        /// <summary>
        /// 充值密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ChangeUserPassword(string id)
        {

            var user = _db.GetTable<Sys_User>().SingleOrDefault(o => o.Id == Int32.Parse(id));
            Random rad = new Random();
            string newPassword = rad.Next(100000, 999999).ToString(CultureInfo.InvariantCulture);
            user.Password = MD5.Encrypt(newPassword, 32);
            _db.SubmitChanges();
            return newPassword;
        }

        private static FileRead fir = new FileRead();
        /// <summary>
        /// 获取日志文件列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetErrorFilesList()
        {
            var dirs = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "/Log");
            var query = from tt in dirs.GetFiles()
                        where tt.Extension != ".ck"
                        orderby tt.LastWriteTime descending
                        select new
                            {
                                tt.Name,
                                Path = tt.FullName
                            };
            return new JsonResult { Data = query.ToList() };
        }

        public string ReadFile(string path)
        {
            lock (fir)
            {


                var f = new FileInfo(path);
                var newpath = f.DirectoryName + @"\" + f.Name.Replace(f.Extension, ".ck");
                if (!File.Exists(newpath))
                {
                    File.Copy(path, newpath, true);
                }
                f = new FileInfo(newpath);
                path = newpath;
                if (fir.Fstream == null)
                {
                    fir = new FileRead
                    {
                        Path = path,
                        Fstream = new StreamReader(path).BaseStream,
                        Length = f.Length
                    }; //生成文件读取进度对象

                }

                lock (f)
                {

                    var buffer = new byte[1024];
                    int length = fir.Fstream.Read(buffer, 0, 1024);
                    fir.Process += length;
                    long process = fir.Process;
                    long fileLength = fir.Length;
                    if (fir.Process == fir.Length) //已经读完了
                    {
                        fir.Fstream.Close(); //关闭读取流
                        fileLength = fir.Length;
                        File.Delete(path);
                        fir = new FileRead();//初始化文件读取进度对象
                    }
                    return
                        Encoding.Default.GetString(buffer, 0, length)
                            .Replace(" ", "&nbsp;") + "$" + process + "$" + fileLength;
                }
            }
        }

        public JsonResult LoadNewList(DataGridParam param)
        {

            param.Query += "Code[Equal]&" + LoginUser.Id;
            return DataSearch<News_User_Company>(param);
        }

        /// <summary>
        /// 在线用户
        /// </summary>
        /// <param name="param">查询对象</param>
        /// <returns></returns>
        public JsonResult OnlineUser(DataGridParam param)
        {
            var qm = param.Query.GetModel();
            qm.Items.Add(new ConditionItem
            {
                Field = "IsSystem",
                Method = QueryMethod.Equal,
                Value = false
            });
            var query = (from tt in UserTicket.Users.AsQueryable().Where(qm)
                         let loger = _db.Sys_PerformLog.Where(c=>c.Controller== "Account"&&c.Action== "Login"&&c.UserId==tt.Id).OrderByDescending(c=>c.CreateTime).FirstOrDefault()
                         let singoutloger = _db.Sys_PerformLog.Where(c => c.Controller == "Home" && c.Action == "Exit" && c.UserId == tt.Id).OrderByDescending(c => c.CreateTime).FirstOrDefault()
                         select new
                         {
                             tt.Id,
                             tt.Name,
                             tt.MyName,
                             IP = loger == null ? null : loger.Ip,
                             CreateTime = loger == null ? null : (DateTime?)loger.CreateTime,
                             SignoutTime = singoutloger == null ? null : (DateTime?)singoutloger.CreateTime,
                         }).OrderBy(param.SortName + " " + param.SortOrder);
            return new JsonResult { Data = query.ToList() };
        }

        public string SingOutUser(string ids)
        {
            var userids = ids.Split(',').GetIntArray();
            foreach (var id in userids)
            {
                lock (MvcApplication.lockObject)
                {
                    UserTicket.Users.RemoveAll(c => c.Id == id);
                }

            }
            return Suggestion.SingOutSucceed;


        }

        public IList IconList()
        {
            var query = from tt in _db.Sys_Icon
                        where tt.Status
                        select new
                        {
                            IconName = tt.IconClass,
                            Title = tt.Name
                        };
            return query.ToList();
        }

        public string CreateOrUpdate<T>(string pkName, dynamic pkValue, dynamic initialValue, T InsertEntity, dynamic updateEntity) where T : class, new()
        {
            return DataControl(pkName, pkValue, initialValue, InsertEntity, updateEntity);
        }
    }
}