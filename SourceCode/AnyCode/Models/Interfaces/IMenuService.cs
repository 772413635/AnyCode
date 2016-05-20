using System;
using System.Collections.Generic;
using Common;
using DBlinq;

namespace AnyCode
{
    public interface ISysMenuService : IDisposable
    {
        List<Menu> GetMenu(List<string> Pid, string fid);
        List<Sys_Menu> GetAll();
        string GetMenuAll();
    }
}