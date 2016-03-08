using System;
using System.Collections.Generic;
using DBlinq;

namespace AnyCode
{
    public interface ISysUserStatusService : IDisposable
    {
        List<Sys_UserStatus> GetAll();
    }
}