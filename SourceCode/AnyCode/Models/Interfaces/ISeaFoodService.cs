using System;
using Common;

namespace AnyCode.Models.Interfaces
{
    interface ISeaFoodService
    {
        /// <summary>
        /// 获取海鲜产品列表
        /// </summary>
        /// <param name="param">查询对象</param>
        /// <returns>海鲜产品数据</returns>
        JqueryGridObject ProductList(DataGridParam param);

        /// <summary>
        /// 初始化微信用户信息
        /// </summary>
        /// <param name="param">传递的对象</param>
        /// <returns>初始化结果</returns>
        bool IntiUser(DataGridParam param);
    }
}
