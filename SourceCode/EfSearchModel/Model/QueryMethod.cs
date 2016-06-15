using System.ComponentModel;

namespace EfSearchModel.Model
{
    /// <summary>
    /// Html表单元素的检索方式
    /// </summary>
    public enum QueryMethod
    {
        /// <summary>
        /// 等于
        /// </summary>
        //[GlobalCode("=", OnlyAttribute = true)]
        Equal = 0,

        /// <summary>
        /// 小于
        /// </summary>
        //// [GlobalCode("<", OnlyAttribute = true)]
        LessThan = 1,

        /// <summary>
        /// 大于
        /// </summary>
        // [GlobalCode(">", OnlyAttribute = true)]
        GreaterThan = 2,

        /// <summary>
        /// 小于等于
        /// </summary>
        // [GlobalCode("<=", OnlyAttribute = true)]
        LessThanOrEqual = 3,

        /// <summary>
        /// 大于等于
        /// </summary>
        // [GlobalCode(">=", OnlyAttribute = true)]
        GreaterThanOrEqual = 4,

        /// <summary>
        /// Like
        /// </summary>
        // [GlobalCode("like", OnlyAttribute = true)]
        Like = 6,

        /// <summary>
        /// In
        /// </summary>
        // [GlobalCode("in", OnlyAttribute = true)]
        In = 7,

        /// <summary>
        /// 输入一个时间获取当前天的时间块操作, ToSql未实现，仅实现了IQueryable
        /// </summary>
        // [GlobalCode("between", OnlyAttribute = true)]
        DateBlock = 8,

        // [GlobalCode("<>", OnlyAttribute = true)]
        NotEqual = 9,


        // [GlobalCode("like", OnlyAttribute = true)]
        StartsWith = 10,

        // [GlobalCode("like", OnlyAttribute = true)]
        EndsWith = 11,

        /// <summary>
        /// 处理Like的问题
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Contains = 12,

        /// <summary>
        /// 处理In的问题
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        StdIn = 13,
        
        /// <summary>
        /// 处理Datetime小于+23h59m59s999f的问题
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        DateTimeLessThanOrEqual = 14
    }
}