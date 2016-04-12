using System;
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
        /// 获取用户默认地址
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Sf_Address DefaultAddress(DataGridParam param)
        {
            var openid = param.Query;
            var address = _db.Sf_Address.SingleOrDefault(c => c.OpenId == openid&&c.IsDefault==true);
            return address;
        }

        /// <summary>
        /// 获取用户收货地址
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public JqueryGridObject AddressList(DataGridParam param)
        {
            var query = (from tt in _db.Sf_Address
                        where tt.OpenId == param.Query
                        select new
                        {
                            tt.Id,
                            tt.Contacts,
                            tt.Phone,
                            tt.IsDefault,
                            tt.Address
                        }).ToList();
            var count = query.Count;
            return new JqueryGridObject
            {
                DataGridParam = param,
                total = count,
                Query = query
            };
        }

        /// <summary>
        /// 更新默认地址
        /// </summary>
        /// <param name="param"></param>
        public void UpdateAddressDefault(DataGridParam param)
        {
            var id = Int32.Parse(param.Query);

            var addressList = _db.GetTable<Sf_Address>().Where(c => c.Id != id);
            var singeAddress = _db.GetTable<Sf_Address>().Single(c => c.Id == id);
            foreach (var address in addressList)
            {
                address.IsDefault = false;
            }
            singeAddress.IsDefault = true;
            _db.SubmitChanges();
        }

        /// <summary>
        /// 初始化微信用户
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool InitUser(DataGridParam param)
        {
            var openid = param.Query;
            var user = _db.Sf_User.FirstOrDefault(c => c.OpenId == openid);
            if (user != null)
            {
                return true;
            }
            else
            {
                var res = DataControl<Sf_User>("Id", 0, 0, new Sf_User { OpenId = openid }, null);
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

        /// <summary>
        /// 添加收货地址，并设置为默认地址
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool AddAddress(DataGridParam param)
        {
            if (!string.IsNullOrWhiteSpace(param.Query))
            {
                var addressInfo = param.Query.Split('&');
                var address = new Sf_Address
                {
                    Contacts=addressInfo[0],
                    Phone=addressInfo[1],
                    Address=addressInfo[2],
                    OpenId=addressInfo[3],
                    IsDefault=true,
                    CreateTime=DateTime.Now

                };
                _db.GetTable<Sf_Address>().InsertOnSubmit(address);
                var query = _db.GetTable<Sf_Address>().SingleOrDefault(c => c.IsDefault == true && c.OpenId == address.OpenId);
                if (query != null)
                {
                    query.IsDefault = false;
                }
                _db.SubmitChanges();
                return true;
            }
            return false;

        }
    }
}