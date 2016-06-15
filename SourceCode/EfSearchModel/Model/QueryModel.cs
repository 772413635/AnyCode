using System.Collections.Generic;

namespace EfSearchModel.Model
{
    /// <summary>
    /// 用户自动收集搜索条件的Model
    /// </summary>
    public class QueryModel
    {
        public QueryModel()
        {
           Items = new List<ConditionItem>();
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        public List<ConditionItem> Items { get; set; }     

    }
}