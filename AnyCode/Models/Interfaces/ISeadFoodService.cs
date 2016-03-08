using System;
using Common;

namespace AnyCode.Models.Interfaces
{
    interface ISeadFoodService
    {
        /// <summary>
        /// 获取海鲜产品列表
        /// </summary>
        /// <param name="param">查询对象</param>
        /// <returns>海鲜产品数据</returns>
        JqueryGridObject ProductList(DataGridParam param);
    }
}
