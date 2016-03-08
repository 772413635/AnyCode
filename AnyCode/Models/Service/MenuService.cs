using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using DBlinq;

namespace AnyCode.Models.Service
{
    public class SysMenuService : BaseService, ISysMenuService
    {
        public SysMenuService(Sys_User loginUser)
            : base(loginUser)
        {
        }

        public List<Menu> GetMenu(List<string> pids, string fid)
        {

            var rootNodes = new List<Menu>();//一级节点
            var roleFunctions = GetPermissionsName(fid.GetPermissionsById());//获得权限字典




            var firstNodeList =
                _db.Sys_Menu.Where(o => o.IsColumn && pids.Contains(o.Id.ToString())).OrderBy(
                    o => o.Sort).ToList();
            if (firstNodeList.Any())
            {

                foreach (var q in firstNodeList)
                {
                    var secondMenus = new List<Menu>(); //二级节点
                    var secondNodeList =
                        _db.Sys_Menu.Where(
                            o =>
                                o.ParentId == q.Id.ToString() && pids.Contains(o.Id.ToString()) && o.State == true &&
                                o.IsColumn == false).OrderBy(o => o.Sort).ToList();
                    foreach (var p in secondNodeList)
                    {
                        string url = p.Url;
                        if (roleFunctions.Keys.Contains(p.Id.ToString()))
                        {
                            url += "?funnames=" + roleFunctions[p.Id.ToString()];
                        }
                        secondMenus.Add(new Menu
                        {
                            MenuId = p.Id,
                            Icon = p.Iconic,
                            Url = url,
                            Remark = p.Remark,
                            MenuName = p.Name
                        });
                    }
                    rootNodes.Add(new Menu
                    {
                        MenuId = q.Id,
                        Icon = q.Iconic,
                        MenuName = q.Name,
                        Menus = secondMenus
                    });
                }
            }
            else
            {
                var secondMenus = new List<Menu>(); //二级节点
                var secondNodeList = _db.Sys_Menu.Where(o => pids.Contains(o.Id.ToString()) && o.State == true && o.IsColumn == false).OrderBy(o => o.Sort).ToList();
                foreach (var p in secondNodeList)
                {
                    string url = p.Url;
                    if (roleFunctions.Keys.Contains(p.Id.ToString()))
                    {
                        url += "?funnames=" + roleFunctions[p.Id.ToString()];
                    }
                    secondMenus.Add(new Menu
                    {
                        MenuId = p.Id,
                        Icon = p.Iconic,
                        Url = url,
                        Remark = p.Remark,
                        MenuName = p.Name
                    });
                }
                rootNodes = secondMenus;
            }
            return rootNodes;


        }


        /// <summary>
        /// 处理权限字符串，id变成名字
        /// </summary>
        /// <param name="permissions">权限字典</param>
        /// <returns></returns>
        private Dictionary<string, string> GetPermissionsName(Dictionary<string, string> permissions)
        {
            var permissionsName = new Dictionary<string, string>();
            foreach (var p in permissions)
            {
                var values = p.Value.Split(';');
                var names = from tt in _db.Sys_Function
                            where values.Contains(tt.Id.ToString()) && tt.Status
                            orderby tt.Sort
                            select new
                            {
                                tt.Function
                            };
                var namestring = Enumerable.Aggregate(names, "", (current, n) => current + (n.Function + ","));
                permissionsName.Add(p.Key, namestring.TrimEnd(','));
            }
            return permissionsName;
        }

        public string GetMenuAll()
        {
            //拼接字符串
            StringBuilder strmenu = new StringBuilder();
            //一级节点
            List<Sys_Menu> firstNodeList = new List<Sys_Menu>();

            //菜单根据权限获得

            firstNodeList =
                _db.Sys_Menu.Where(o => o.IsColumn).OrderBy(
                    o => o.Sort).ToList();
            //二级节点
            int flag = 0;
            if (firstNodeList.Count > 0)
            {
                strmenu.Append("{@menus@:[");
                foreach (Sys_Menu s in firstNodeList)
                {
                    flag++;
                    string str = "{@menuid@:@" + s.Id + "@,@icon@:@" + s.Iconic + "@,@menuname@:@" + s.Name +
                                 "@,@menus@:[";
                    strmenu.Append(str);
                    List<Sys_Menu> secondNodeList;
                    //secondNodeList = objDataContext.Sys_Menu.Where(o => o.ParentId == s.Id  && LoginUser.Pid.Split(',').Contains(o.Id)).OrderBy(o => o.Sort).ToList();
                    secondNodeList =
                        _db.Sys_Menu.Where(o => o.ParentId == s.Id.ToString()).OrderBy(o => o.Sort).ToList();
                    int flag2 = 0;
                    foreach (Sys_Menu s1 in secondNodeList)
                    {
                        flag2++;
                        string str2 = "{@menuid@:@" + s1.Id + "@,@icon@:@" + s1.Iconic + "@,@url@:@" + s1.Url +
                                      "@,@menuname@:@" + s1.Name + "@";
                        strmenu.Append(str2);
                        strmenu.Append(@"}");
                        if (flag2 != secondNodeList.Count)
                        {
                            strmenu.Append(@",");
                        }
                    }
                    strmenu.Append(@"]}");
                    if (flag != firstNodeList.Count)
                    {
                        strmenu.Append(@",");
                    }
                }
                strmenu.Append("]}");
                return strmenu.ToString().Replace('@', '"');
            }
            return string.Empty;
        }
        public List<Sys_Menu> GetAll()
        {
            try
            {
                List<Sys_Menu> list = _db.Sys_Menu.ToList();
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}