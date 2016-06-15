using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBlinq;
using LinqTODB;

namespace GodBuild
{
    public sealed class ServiceBuilder
    {
        //权限
        public static ISysMenuService BuildSysMenuService(Sys_User loginUser)
        {
            return new SysMenuService(loginUser);
        }
        //系统功能
        public static ISysSystem BuildSysSystem(LinqToDB context, Sys_User loginUser)
        {
            return new SysSystem(context, loginUser);
        }
    }
}