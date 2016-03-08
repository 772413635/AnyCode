using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EfSearchModel.Common
{
    /// <summary>
    /// MvcHtmlString包装器
    /// 用于设置对Html的替换规则并在输出时统一处理显示Html
    /// </summary>
    public class MvcHtmlWrapper : IHtmlString
    {
        /// <summary>
        /// 构建一个MvcHtmlWrapper，如果是MvcHtmlWrapper则直接返回，
        /// 如果是是其它类型则构造MvcHtmlWrapper后返回
        /// </summary>
        /// <param name="str">IHtmlString类型，实现了ToHtmlString接口的类型</param>
        /// <returns></returns>
        public static MvcHtmlWrapper Create(IHtmlString str)
        {
            Contract.Requires(str != null);
            if (str is MvcHtmlWrapper)
                return str as MvcHtmlWrapper;
            if (str is MvcHtmlString)
                return new MvcHtmlWrapper(str);
            Contract.Assert(false);
            return null;
        }

        IHtmlString HtmlStringInterface { get; set; }

        private string _htmlString;

        /// <summary>
        /// 获取MvcHtmlString所生成的Html字符串
        /// </summary>
        public string HtmlString
        {
            get { return _htmlString ?? (_htmlString = HtmlStringInterface.ToHtmlString()); }
        }

        /// <summary>
        /// 用于替换的Dict
        /// </summary>
        public List<Tuple<string, string>> ReplaceDict { get; set; }

        /// <summary>
        /// 构造MvcHtmlString包装器
        /// </summary>
        /// <param name="str">MvcHtmlString的实例，不允许为空</param>
        MvcHtmlWrapper(IHtmlString str)
        {
            Contract.Requires(str != null);
            HtmlStringInterface = str;
            ReplaceDict = new List<Tuple<string, string>>();
        }
 
        /// <summary>
        /// 对ToHtmlString进行了重写， 输出HtmlString的内容，并按替换规则进行了替换
        /// </summary>
        /// <returns></returns>
        public string ToHtmlString()
        {
            return ToString();
        }

        /// <summary>
        /// 对ToString进行了重写， 输出HtmlString的内容，并按替换规则进行了替换
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder(HtmlString);
            foreach (var item in ReplaceDict)
            {
                if (!string.IsNullOrEmpty(item.Item1))
                {
                    sb.Replace(item.Item1, item.Item2);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 添加替换原则
        /// </summary>
        /// <param name="item1">被替换的字符串</param>
        /// <param name="item2">替换为的字符串</param>
        internal void Add(string item1, string item2)
        {
            ReplaceDict.Add(Tuple.Create(item1, item2));
        }
    }
}