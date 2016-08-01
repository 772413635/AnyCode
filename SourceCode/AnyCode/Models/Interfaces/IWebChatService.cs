using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common;
using Senparc.Weixin.Entities;

namespace AnyCode.Models.Interfaces
{
    public interface IWebChatService
    {
        JqueryGridObject WxCfgList(DataGridParam param);

        /// <summary>
        /// 删除微信账号
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        string DeleteAccount<T>(string id) where T : class, new();

        string CreateOrUpdate<T>(string pkName, dynamic pkValue, dynamic initialValue, T InsertEntity, dynamic updateEntity) where T : class, new();

        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="ajaxValidate">验证对象</param>
        /// <returns>验证结果</returns>
        AjaxValidateResult SingleToken(AjaxValidate ajaxValidate);

        /// <summary>
        /// 删除微信菜单
        /// </summary>
        /// <param name="menuid">菜单id</param>
        /// <returns></returns>
        string DelteMenu(string menuid);


        /// <summary>
        /// 重命名菜单
        /// </summary>
        /// <param name="menuid">菜单id</param>
        /// <param name="newName">菜单id</param>
        /// <returns></returns>
        string ReName(int menuid, string newName);
        /// <summary>
        /// 添加一级菜单
        /// </summary>
        /// <param name="name">菜单名</param>
        /// <param name="cfgId">公众号值</param>
        /// <returns></returns>
        string AddFartherMenu(string name, int cfgId);

        /// <summary>
        /// 更新一级菜单
        /// </summary>
        /// <param name="id">菜单id</param>
        /// <returns></returns>
        string UpdateFartherMenu(int id);
        string AddChildrenMenu(string name, int cfgId, int fid);
        string SetChildrenBtn(int id, int type, string val);

        dynamic Menu(int configId);
        WxJsonResult CreateMenu(int cfgid, string token);

        JqueryGridObject MsgMapList(DataGridParam param);
    }
}
