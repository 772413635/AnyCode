
using System;

namespace EfSearchModel.Model
{
    /// <summary>
    /// 用于存储查询条件的单元
    /// </summary>
    public class ConditionItem
    {
        public ConditionItem(){}

        public ConditionItem(string name ,string value)
        {
            string [] ss=name.Split(',');
            Field = ss[2];
            Method = (QueryMethod)Enum.Parse(typeof(QueryMethod), ss[0]);
            Value = value.TrimStart('[').TrimEnd(']');
        }

        public ConditionItem(string field, QueryMethod method, object val)
        {
            Field = field;
            Method = method;
            Value = val;
        }

        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }
        
        /// <summary>
        /// 查询方式，用于标记查询方式HtmlName中使用[]进行标识
        /// </summary>
        public QueryMethod Method { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 前缀，用于标记作用域，HTMLName中使用()进行标识
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 如果使用Or组合，则此组组合为一个Or序列
        /// </summary>
        public string OrGroup { get; set; }
    }
}
