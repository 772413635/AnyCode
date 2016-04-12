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
        /// 获取用户的收货地址
        /// </summary>
        /// <param name="param">查询对象</param>
        /// <returns></returns>
        JqueryGridObject AddressList(DataGridParam param);

        /// <summary>
        /// 获取用户默认地址
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        DBlinq.Sf_Address DefaultAddress(DataGridParam param);

        /// <summary>
        /// 更新默认地址
        /// </summary>
        /// <param name="param"></param>
        void UpdateAddressDefault(DataGridParam param);

        /// <summary>
        /// 初始化微信用户信息
        /// </summary>
        /// <param name="param">传递的对象</param>
        /// <returns>初始化结果</returns>
        bool InitUser(DataGridParam param);

        /// <summary>
        /// 添加地址并设置为默认地址
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        bool AddAddress(DataGridParam param);
    }
}
