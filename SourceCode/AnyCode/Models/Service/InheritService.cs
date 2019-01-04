using System;
using System.Linq;
using System.Linq.Dynamic;
using Common;
using DBlinq;
using EfSearchModel;
using EfSearchModel.Model;
using AnyCode.Models.Interfaces;

namespace AnyCode.Models.Service
{
    public class InheritService : BaseService, IInheritService
    {
        public InheritService(LinqToDB db, Sys_User loginuser)
            : base(db, loginuser)
        {

        }

        public JqueryGridObject GetList<T>(DataGridParam param) where T : class, new()
        {
            var qm = param.Query.GetModel();
            var query = from tt in _db.GetTable<T>().Where(qm).OrderBy(param.SortName + " " + param.SortOrder)
                        select tt;
            return new JqueryGridObject
            {
                DataGridParam = param,
                total = query.Count(),
                Query = query.Skip((param.Page - 1) * param.RP).Take(param.RP)
            };
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="id">数据id集合</param>
        /// <returns></returns>
        public string Delete<T>(string id) where T : class, new()
        {
            return DeleteData<T>(id);
        }

        public string CreateOrUpdate<T>(string pkName, dynamic pkValue, dynamic initialValue, T InsertEntity, dynamic upldateEntity) where T : class,new()
        {
            return DataControl(pkName, pkValue, initialValue, InsertEntity, upldateEntity);
        }
    }
}