using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Common;
using DBlinq;
using EfSearchModel;
using EfSearchModel.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace AnyCode
{

    public abstract class BaseService
    {



        protected readonly LinqToDB _db;
        private readonly Sys_User _loginUser;

        protected BaseService(LinqToDB db, Sys_User loginUser)
        {
            _db = db;
            _loginUser = loginUser;

        }
        protected BaseService(Sys_User loginUser)
        {
            _db = new LinqToDB();
            _loginUser = loginUser;
        }

        public Sys_User LoginUser
        {
            get { return _loginUser; }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="param">查询对象</param>
        /// <returns></returns>
        protected JsonResult DataSearch<T>(DataGridParam param) where T : class,new()
        {
            var dc = _db;
            int rp = param.RP;
            int page = param.Page;
            QueryModel qm = param.Query.GetModel();
            var query = dc.GetTable<T>().Where(qm);
            int count = query.Count();
            if (!string.IsNullOrEmpty(param.SortName))
            {
                query = query.OrderBy(param.SortName + " " + param.SortOrder);
            }
            query = query.Skip((page - 1) * rp).Take(rp);
            var jq = new JqueryGridObject
            {
                DataGridParam = param,
                total = count,
                Query = query
            };
            return new JsonResult { Data = jq };
        }

        protected JsonResult MongoDataSearch<T>(DataGridParam param) where T : class,new()
        {
            var db = MongoDbHelper.DataBase;

            IMongoSortBy sort = null;
            if (param.SortOrder == "asc")
            {
                sort = SortBy.Ascending(param.SortName);
            }
            else if (param.SortOrder == "desc")
            {
                sort = SortBy.Descending(param.SortName);
            }
            var data = db.GetCollection<T>(typeof(T).Name).FindAs<T>(null).SetSortOrder(sort);
            var jq = new JqueryGridObject
            {
                DataGridParam = param,
                total = (int)data.Count(),
                Query = data.Skip((param.Page - 1) * param.RP).Take(param.RP).ToList()
            };
            return new JsonResult
            {
                Data = jq
            };

        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="id">一组可以转换为整形数组的字符串</param>
        /// <returns></returns>
        protected string DeleteData<T>(string id) where T : class ,new()
        {
            var tb = _db;
            var queryMatch = new QueryModel();
            var items = new ConditionItem
                {
                    Field = "Id",
                    Method = QueryMethod.StdIn,
                    Value = id.Split(',').GetIntArray()
                };
            queryMatch.Items.Add(items);
            var query = tb.GetTable<T>().Where(queryMatch);
            tb.DeleteAllOnSubmit(query);
            tb.SubmitChanges();
            return Suggestion.DeleteSucceed;


        }


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="id">一组可以转换为字符串数组的字符串</param>
        /// <returns></returns>
        protected string DeleteData_string<T>(string id) where T : class ,new()
        {
            var tb = _db;
            var queryMatch = new QueryModel();
            var items = new ConditionItem
            {
                Field = "Id",
                Method = QueryMethod.StdIn,
                Value = id.Split(',')
            };
            queryMatch.Items.Add(items);
            var query = tb.GetTable<T>().Where(queryMatch);
            tb.DeleteAllOnSubmit(query);
            tb.SubmitChanges();
            return Suggestion.DeleteSucceed;


        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="id">主键值</param>
        /// <param name="t">更新model</param>
        /// <returns></returns>
        protected string UpdateData<T>(string id, T t) where T : class, new()
        {
            var queryMatch = new QueryModel();
            var items = new ConditionItem
                {
                    Field = "Id",
                    Method = QueryMethod.Equal,
                    Value = id
                };
            queryMatch.Items.Add(items);
            var query = _db.GetTable<T>().Where(queryMatch).Single();
            var properties = t.GetType().GetProperties();
            foreach (var p in properties)
            {
                if (p.Name == "Version" || p.Name == "Id" || p.Name.IndexOf('_') > 0)
                    continue;
                query.GetType().GetProperty(p.Name).SetValue(query, p.GetValue(t, null), null);
            }
            _db.SubmitChanges();
            return Suggestion.UpdateSucceed;

        }

        /// <summary>
        /// 更新||添加数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="pkName">主键名</param>
        /// <param name="pkValue">主键值</param>
        /// <param name="initialValue">id初始值，若主键值==初始值，则添加，否则更新</param>
        /// <param name="InsertEntity">新增数据实体</param>
        /// <param name="updateEntity">更新数据实体</param>
        /// <returns></returns>
        public string DataControl<T>(string pkName, dynamic pkValue, dynamic initialValue, T InsertEntity, dynamic updateEntity) where T : class, new()
        {
            var l2Db = new LinqToDB();
            var queryMatch = new QueryModel();
            var item = new ConditionItem();
            //添加数据
            if (pkValue == initialValue || string.IsNullOrWhiteSpace(pkValue.ToString()))
            {
                var properties = InsertEntity.GetType().GetProperties();
                if (properties.Any(c => c.Name == "CreateTime"))
                {
                    InsertEntity.GetType().GetProperty("CreateTime").SetValue(InsertEntity, DateTime.Now, null);
                }
                if (properties.Any(c => c.Name == "CreatePerson"))
                {
                    InsertEntity.GetType().GetProperty("CreatePerson").SetValue(InsertEntity, LoginUser.Id, null);
                }

                l2Db.InsertOnSubmit(InsertEntity);
                l2Db.SubmitChanges();
                return Suggestion.InsertSucceed;
            }
            //更新数据
            item.Field = pkName;
            item.Method = QueryMethod.Equal;
            item.Value = pkValue;
            queryMatch.Items.Add(item);
            T update = l2Db.GetTable<T>().Where(queryMatch).Single();//查询出实体
            var uproperties = update.GetType().GetProperties();//获取update实体的所有属性
            LinqChange.CopyObjectProperty(update, updateEntity);//根据匿名类型实体updateEntity给update实体属性赋值
            if (uproperties.Any(c => c.Name == "UpdateTime"))
            {
                update.GetType().GetProperty("UpdateTime").SetValue(update, DateTime.Now, null);
            }
            if (uproperties.Any(c => c.Name == "UpdatePerson"))
            {
                update.GetType().GetProperty("UpdatePerson").SetValue(update, LoginUser.Id, null);
            }
            l2Db.SubmitChanges();
            return Suggestion.UpdateSucceed;

        }

        public void Dispose()
        {
        }
    }

    //扩展html.lable
    public static class HttpHelper
    {
        public static MvcHtmlString LableValueFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var name = metadata.PropertyName;
            TModel t = htmlHelper.ViewData.Model;
            if (t != null)
            {
                string value = t.GetType().GetProperty(name).GetValue(t, null).ToString();
                return MvcHtmlString.Create("<lable>" + value + "</lable>");
            }
            return MvcHtmlString.Create("<lable></lable>");
        }

        public static MvcHtmlString LableValueFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                                     Expression
                                                                         <Func<TModel, TProperty>> expression,
                                                                     object htmlCss)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var name = metadata.PropertyName;
            TModel t = htmlHelper.ViewData.Model;
            var css = htmlCss.ToString();
            if (t != null)
            {
                string value = t.GetType().GetProperty(name).GetValue(t, null).ToString();
                return MvcHtmlString.Create("<lable " + css + ">" + value + "</lable>");
            }
            return MvcHtmlString.Create("<lable " + css + "></lable>");
        }



        public static MvcHtmlString TextValueFor<TModel, TProperty>(
        this HtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TProperty>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var name = metadata.PropertyName;
            TModel t = htmlHelper.ViewData.Model;
            if (t != null)
            {
                string value = t.GetType().GetProperty(name).GetValue(t, null).ToString();
                value = value.Replace("\n", "<br/>");
                return MvcHtmlString.Create(value);
            }
            return MvcHtmlString.Create("");
        }

        public static MvcHtmlString TextValueFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                                     Expression
                                                                         <Func<TModel, TProperty>> expression,
                                                                     object htmlCss)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var name = metadata.PropertyName;
            TModel t = htmlHelper.ViewData.Model;
            var css = htmlCss.ToString();
            if (t != null)
            {
                string value = t.GetType().GetProperty(name).GetValue(t, null).ToString();
                value = value.Replace("\n", "<br/>");
                return MvcHtmlString.Create(value);
            }
            return MvcHtmlString.Create("");
        }

    }


    //日期扩展
    public static class DateFormatter
    {
        /// <summary>
        /// 返回dateTime的字符串(yyyy-MM-dd HH:mm:ss)格式
        /// </summary>
        /// <param name="dt">dateTime</param>
        /// <returns></returns>
        public static string toFormatter(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回日期的下个月的第一天的凌晨
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime AddMonthFirstDay(this DateTime dt)
        {
            DateTime newdate = dt.Date.AddMonths(1);//加一个月
            int year = newdate.Year;
            int month = newdate.Month;
            newdate = DateTime.Parse(year + "-" + month + "-01" + " 00:00:00");
            return newdate;
        }

        /// <summary>
        /// 返回当前月份最后一天的最后时刻
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime ThisDateLastDay(this DateTime dt)
        {
            DateTime newdate = dt.Date.AddMonths(1).Date;//下一个月的日期
            newdate = DateTime.Parse(newdate.Year + "-" + newdate.Month + "-01");//下一个月的第一天
            newdate = newdate.AddDays(-1);//获得前一天
            newdate = DateTime.Parse(newdate.Year + "-" + newdate.Month + "-" + newdate.Day + " 23:59:59");
            return newdate;
        }

        /// <summary>
        /// 返回当天的最后时刻
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime thisDayLastTime(this DateTime dt)
        {
            DateTime newdt;
            newdt = dt.Date.AddDays(1).AddSeconds(-1);
            return newdt;
        }

        public static int[] GetIntArray(this string[] sds)
        {
            var result = new List<int>();
            foreach (var s in sds)
            {
                int oo = 0;
                if (int.TryParse(s, out oo))
                {
                    result.Add(oo);
                }
            }
            return result.ToArray();

        }

        public static QueryModel GetModel(this string ts)
        {
            var model = new QueryModel();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic = ValueConvert.StringToDictionary(ts);
            foreach (var d in dic)
            {
                var cdn = new ConditionItem();
                string meth = Regex.Match(d.Key, @"\[\w*\]").ToString();
                cdn.Value = d.Value;
                cdn.Method = (QueryMethod)Enum.Parse(typeof(QueryMethod), meth.TrimStart('[').TrimEnd(']'));
                cdn.Field = d.Key.Replace(meth, "");
                model.Items.Add(cdn);
            }
            return model;
        }
    }


}