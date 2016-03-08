using Common;
using DBlinq;

namespace AnyCode
{
    public interface ISysExceptionService
    {
        bool Add(Sys_Exception entity);
      
        bool Add(ref ValidationErrors validationErrors, Sys_Exception entity);
        
    }
}