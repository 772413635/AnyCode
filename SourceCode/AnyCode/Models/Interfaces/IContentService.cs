using Common;
using DBlinq;

namespace AnyCode.Models.Interfaces
{
    public interface IContentService
    {
        JqueryGridObject TextList(DataGridParam param);
        JqueryGridObject PwdList(DataGridParam param);
        string ShowContent(int id);

        bool JJobMessage(JB_JobMessage message);

        string DeleteText<T>(string id) where T : class, new();
        string DeletePwd<T>(string id) where T : class, new();
        
        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="pkName">主键名</param>
        /// <param name="pkValue">主键值</param>
        /// <param name="initialValue">初始值</param>
        /// <param name="InsertEntity">添加实体</param>
        /// <param name="updateEntity">更新实体</param>
        /// <returns></returns>
        string CreateOrUpdate<T>(string pkName, dynamic pkValue, dynamic initialValue, T InsertEntity, dynamic updateEntity) where T : class, new();
    }
}
