using System;
using System.Collections;
using System.Web.Mvc;
using Common;

namespace AnyCode
{
    public interface ISysSystem:IDisposable
    {
        JsonResult GetData<T>(DataGridParam param) where T : class, new();
        JsonResult GetPerformLogList(DataGridParam param);
        string SystemDeleteData<T>(string id) where T : class, new();
        string SystemDeleteData_string<T>(string id) where T : class, new();
        JsonResult GetColumn(string id);
        JsonResult GetUserList(string id);
        string EditUserPwd<T>(string id, T t) where T : class, new();
        JsonResult GetRoleList(string id);
        JsonResult GetCompetenceByRoleId(string id);
        JsonResult GetCompetencebyUserId(string id);
        JsonResult GetSystemSelectTree(string id);
        JqueryGridObject GetUserTable(string id, DataGridParam param);
        JqueryGridObject GetRoleTable(string id, DataGridParam param);
        string ChangeUserPassword(string id);
        JsonResult GetErrorFilesList();
        string ReadFile(string path);
        JsonResult LoadNewList(DataGridParam param);

        /// <summary>
        /// 查询在线用户
        /// </summary>
        /// <param name="param">查询对象</param>
        /// <returns></returns>
        JsonResult OnlineUser(DataGridParam param);

        /// <summary>
        /// 强制退出用户
        /// </summary>
        /// <param name="ids">用户id集合</param>
        /// <returns>强退结果</returns>
        string SingOutUser(string ids);
        IList IconList();

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