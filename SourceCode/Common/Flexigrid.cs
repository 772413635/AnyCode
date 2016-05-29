using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Common
{
    [DataContract]
    public class FlexigridRow
    {


        [DataMember]
        public List<string> cell = new List<string>();
        [OperationContract]
        public static List<string> GetPropertyList(object obj, string[] Cols)
        {
            List<string> propertyList = new List<string>();
            Dictionary<string, string> propertyName = new Dictionary<string, string>();
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                object o = property.GetValue(obj, null);
                if (!string.IsNullOrEmpty(property.Name) && o != null)
                {
                    propertyName.Add(property.Name, o.ToString());
                }
            }

            Cols.All(a =>
            {
                if (propertyName.ContainsKey(a))
                {
                    propertyList.Add(propertyName[a]);
                }
                else
                {
                    propertyList.Add("");
                }
                return true;
            });
            return propertyList;
        }
        [OperationContract]
        public static List<string> GetPropertyList(object obj, string[] Cols, int MySeq)
        {
            List<string> propertyList = new List<string>();
            Dictionary<string, string> propertyName = new Dictionary<string, string>();
            Type type = obj.GetType();//5+1+a+s+p+x
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                object o = property.GetValue(obj, null);
                if (!string.IsNullOrEmpty(property.Name) && o != null)
                {
                    propertyName.Add(property.Name, o.ToString());
                }
            }

            Cols.All(a =>
            {
                if (!string.IsNullOrWhiteSpace(a) && a == "MySeq")
                {
                    propertyList.Add(MySeq.ToString());
                    return true;
                }
                if (propertyName.ContainsKey(a))
                {

                    propertyList.Add(propertyName[a]);
                }
                else
                {
                    propertyList.Add("");
                }
                return true;
            });
            return propertyList;
        }
    }
    /// <summary>
    /// 将数据转换成Flaxigrid认识格式
    /// </summary>
    [DataContract]
    public class FlexigridObject
    {
        [DataMember]
        public FlexigridParam FlexigridPara { get; set; }
        [DataMember]
        public List<FlexigridRow> rows = new List<FlexigridRow>();
        [DataMember]
        public int page = 1;
        [DataMember]
        public int total { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        [DataMember]
        public IQueryable<dynamic> Query
        {
            set
            {
                if (value != null)
                {
                    int MySeq = 1;
                    page = FlexigridPara.Page;
                    if (page > 1)
                    {
                        MySeq = (page - 1) * FlexigridPara.RP + 1;
                    }

                    foreach (dynamic item in value)
                    {
                        FlexigridRow cell = new FlexigridRow
                        {

                            //Id = item.Id.ToString(),
                            cell = FlexigridRow.GetPropertyList(item, FlexigridPara.Cols.Split(','), MySeq)
                        };
                        rows.Add(cell);
                        MySeq++;
                    }
                }
            }
        }
    }
    /// <summary>
    ///  flexigrid与后台传输的对象
    /// </summary>
    [DataContract]
    public class FlexigridParam
    {
        /*  js中 参数使用
         var param = [
					 { name : 'page', value : p.newp }
					,{ name : 'rp', value : p.rp }
					,{ name : 'sortname', value : p.sortname}
					,{ name : 'sortorder', value : p.sortorder }
					,{ name : 'query', value : p.query}
					,{ name : 'qtype', value : p.qtype}
				];							 
         */
        [DataMember]
        public int Page { get; set; }
        [DataMember]
        public int RP { get; set; }
        [DataMember]
        public string SortName { get; set; }
        [DataMember]
        public string SortOrder { get; set; }
        [DataMember]
        public string Query { get; set; }
        [DataMember]
        public string QType { get; set; }
        [DataMember]
        public string Cols { get; set; }
    }
}
