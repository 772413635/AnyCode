using System;
using System.Collections.Generic;
using System.Linq;
using DBlinq;

namespace AnyCode
{
    public class SysUserStatusService : BaseService, ISysUserStatusService
    {
        private readonly LinqToDB objDataContext;

        public SysUserStatusService(Sys_User loginUser)
            : base(loginUser)
        {
            objDataContext = new LinqToDB();
        }
        public List<Sys_UserStatus> GetAll()
        {
            try
            {
                List<Sys_UserStatus> list = objDataContext.Sys_UserStatus.OrderBy(model => model.Id).ToList();
                return list;
            }
            catch (Exception ex) { throw ex; }
        }

    }
}