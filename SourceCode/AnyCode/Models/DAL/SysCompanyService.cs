using DBlinq;

namespace AnyCode.Models.DAL
{
    public class SysCompanyService : BaseService, ISysCompanyService
    {
        private readonly LinqToDB objDataContext;

        public SysCompanyService(LinqToDB context, Sys_User loginuser)
            : base(context, loginuser)
        {

            objDataContext = context;
        }
    }
}