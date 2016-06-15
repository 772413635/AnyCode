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
    public class ContentService : BaseService, IContentService
    {
        protected readonly Sys_User dd = null;
        public ContentService(LinqToDB db, Sys_User loginuser)
            : base(db, loginuser)
        {
        }
        public JqueryGridObject TextList(DataGridParam param)
        {
            var qm = param.Query.GetModel();
            if (!LoginUser.IsSystem)
            {
                qm.Items.Add(new ConditionItem
                {
                    Field = "CreatePerson",
                    Method = QueryMethod.Equal,
                    Value = LoginUser.Id
                });
            }
            var query = from tt in _db.Sys_Text.Where(qm).OrderBy(param.SortName + " " + param.SortOrder)
                        join kk in _db.Sys_User on tt.CreatePerson equals kk.Id into gg
                        from mm in gg.DefaultIfEmpty()
                        select new
                        {
                            tt.Id,
                            tt.Title,
                            tt.CreateTime,
                            mm.MyName
                        };
            return new JqueryGridObject
            {
                DataGridParam = param,
                total = query.Count(),
                Query = query.Skip((param.Page - 1) * param.RP).Take(param.RP)
            };
        }

        public JqueryGridObject PwdList(DataGridParam param)
        {
            var qm = param.Query.GetModel();
            var query = from tt in _db.Sys_Password.Where(qm).OrderBy(param.SortName + " " + param.SortOrder)
                        select new
                        {
                            tt.Id,
                            tt.Name,
                            tt.UserName,
                            tt.Password,
                            tt.Remark,
                            tt.CreateTime
                        };
            return new JqueryGridObject
            {
                DataGridParam = param,
                total = query.Count(),
                Query = query.Skip((param.Page - 1) * param.RP).Take(param.RP)
            };
        }

        /// <summary>
        /// 显示内容
        /// </summary>
        /// <param name="id">数据id</param>
        /// <returns></returns>
        public string ShowContent(int id)
        {
            var text = _db.Sys_Text.SingleOrDefault(c => c.Id == id);
            return text == null ? "" : text.Content;
        }

        public bool JJobMessage(JB_JobMessage message)
        {
            try
            {
                _db.InsertOnSubmit(message);
                _db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="id">数据id集合</param>
        /// <returns></returns>
        public string DeleteText<T>(string id) where T : class, new()
        {
            return DeleteData<T>(id);
        }

        public string DeletePwd<T>(string id) where T : class, new()
        {
            return DeleteData<T>(id);
        }

        public string CreateOrUpdate<T>(string pkName, dynamic pkValue, dynamic initialValue, T InsertEntity, dynamic upldateEntity) where T : class,new()
        {
            return DataControl(pkName, pkValue, initialValue, InsertEntity, upldateEntity);
        }
    }
}