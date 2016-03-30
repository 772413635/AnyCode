﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AnyCode.Models.Interfaces;
using Common;
using DBlinq;
using EfSearchModel;

namespace AnyCode.Models.Service
{
    public class SeaFoodService : BaseService, ISeaFoodService
    {
        public SeaFoodService(LinqToDB db, Sys_User loginUser)
            : base(db, loginUser)
        {
        }

        /// <summary>
        /// 获取海鲜产品
        /// </summary>
        /// <param name="param">查询对象</param>
        /// <returns>海鲜产品数据</returns>
        public JqueryGridObject ProductList(DataGridParam param)
        {
            var qm = param.Query.GetModel();
            var query = from tt in _db.Sf_Product.Where(qm)
                        orderby tt.CreateTime ascending
                        select new
                        {
                            tt.Id,
                            tt.Name,
                            tt.Price,
                            tt.Oprice,
                            tt.Image,
                            tt.Sales,
                            tt.Tag
                        };
            var count = query.Count();
            return new JqueryGridObject
            {
                DataGridParam = param,
                total = count,
                Query = query.Skip((param.Page - 1) * param.RP).Take(param.RP)
            };
        }

        /// <summary>
        /// 初始化微信用户
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool IntiUser(DataGridParam param)
        {
            var openid = param.Query;
            var user = _db.Sf_User.FirstOrDefault(c => c.OpenId == openid);
            if (user != null)
            {
                return true;
            }
            else
            {
                var res=DataControl<Sf_User>("Id", 0, 0, new Sf_User { OpenId = openid }, null);
                if (res == Suggestion.InsertSucceed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}