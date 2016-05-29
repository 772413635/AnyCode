using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System.Web;
using EfSearchModel.Common;
using EfSearchModel.Model;

namespace EfSearchModel.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class MvcHtmlStringExtension
    {
        #region ForSearch

        /// <summary>
        /// 为当前表单元素添加搜索条件
        /// </summary>
        /// <param name="str"></param>
        /// <param name="method">搜索方法</param>
        /// <param name="prefix">前缀</param>
        /// <param name="hasId">是否显示Id，默认false</param>
        /// <param name="orGroup">如果想要支援Or，请设置一个Or分组</param>
        /// <returns></returns>
        public static MvcHtmlWrapper ForSearch(this IHtmlString str, QueryMethod? method, string prefix = "", bool hasId = false, string orGroup = "")
        {
            var wrapper = MvcHtmlWrapper.Create(str);
            Contract.Assert(null != wrapper);
            if (!method.HasValue) return wrapper;
            var html = wrapper.HtmlString;
            #region 如果是CheckBox，则去掉hidden

            if (html.Contains("type=\"checkbox\""))
            {
                var checkMatch = Regex.Match(html, "<input name=\"[^\"]+\" type=\"hidden\" [^>]+ />");
                if (checkMatch.Success)
                {
                    wrapper.Add(checkMatch.Groups[0].Value, string.Empty);
                }
            }

            #endregion

            #region 替换掉Name
            var match = Regex.Match(html, "name=\"(?<name>[^\"]+)\"");
            var strInsert = "";
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                strInsert += string.Format("({0})", prefix);
            }
            if (!string.IsNullOrWhiteSpace(orGroup))
            {
                strInsert += string.Format("{{{0}}}", orGroup);
            }
            if (match.Success)
            {
                wrapper.Add(match.Groups[0].Value,
                            string.Format("name=\"[{1}]{2}{0}\"", match.Groups[1].Value, method, strInsert));
            }

            #endregion

            return wrapper;
        }

        public static MvcHtmlWrapper ForSearch2(this IHtmlString str, QueryMethod? method, string prefix = "", bool hasId = false, string orGroup = "")
        {
            var wrapper = MvcHtmlWrapper.Create(str);
            Contract.Assert(null != wrapper);
            if (!method.HasValue) return wrapper;
            var html = wrapper.HtmlString;
            #region 如果是CheckBox，则去掉hidden

            if (html.Contains("type=\"checkbox\""))
            {
                var checkMatch = Regex.Match(html, "<input name=\"[^\"]+\" type=\"hidden\" [^>]+ />");
                if (checkMatch.Success)
                {
                    wrapper.Add(checkMatch.Groups[0].Value, string.Empty);
                }
            }

            #endregion

            #region 替换掉Name
            var match = Regex.Match(html, "name=\"(?<name>[^\"]+)\"");
            var strInsert = "";
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                strInsert += string.Format("({0})", prefix);
            }
            if (!string.IsNullOrWhiteSpace(orGroup))
            {
                strInsert += string.Format("{{{0}}}", orGroup);
            }
            if (match.Success)
            {
                wrapper.Add(match.Groups[0].Value,
                            string.Format("name=\"[{1}],{2},{0}\"", match.Groups[1].Value, method, strInsert));
            }

            #endregion

            return wrapper;
        }
        #endregion
    }
}
