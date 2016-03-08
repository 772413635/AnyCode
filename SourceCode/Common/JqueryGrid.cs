using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Common
{
    [DataContract]
    public class JqueryGridRow
    {

        [DataMember]
        public Dictionary<string, string> cc = new Dictionary<string, string>();
        [OperationContract]
        public static List<string> GetPropertyList(object obj, int MySeq)
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

            return propertyList;
        }
    }
    /// <summary>
    /// 将数据转换成Flaxigrid认识格式
    /// </summary>
    [DataContract]
    public class JqueryGridObject
    {

        public DataGridParam DataGridParam { get; set; }
        [DataMember]
        public IEnumerable<dynamic> rows;
        [DataMember]
        public int page = 1;
        [DataMember]
        public int total { get; set; }
        [DataMember]
        public List<Dictionary<string, string>> footer = new List<Dictionary<string, string>>();

        /// <summary>
        /// 数据源
        /// </summary>
        /// 

        [DataMember]
        public IEnumerable<dynamic> Query
        {
            set { rows = value; }
        }

    }

    [DataContract]
    public class JqueryGridTreeObject
    {

        public DataGridParam DataGridParam { get; set; }
        [DataMember]
        public List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
        [DataMember]
        public int page = 1;
        [DataMember]
        public int total { get; set; }



        /// <summary>
        /// 数据源
        /// </summary>
        /// 

        [DataMember]
        public IEnumerable<dynamic> Query
        {
            set
            {
                if (value != null)
                {
                    int MySeq = 1;
                    page = DataGridParam.Page;
                    if (page > 1)
                    {
                        MySeq = (page - 1) * DataGridParam.RP + 1;
                    }

                    foreach (dynamic item in value)
                    {

                        Dictionary<string, string> row = new Dictionary<string, string>();
                        row = this.GetPropertyList(item, MySeq);
                        rows.Add(row);
                        MySeq++;
                    }
                }
            }
        }




        [OperationContract]
        public Dictionary<string, string> GetPropertyList(object obj, int MySeq)
        {
            // List<string> propertyList = new List<string>();
            Dictionary<string, string> propertyName = new Dictionary<string, string>();
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                // string proStr = property.GetString();
                // if(proStr.Contains("jiuweiCRM.DBLinq"))
                // {

                // }
                //if (property.GetString().Contains("SysContact"))
                //{
                object o = property.GetValue(obj, null);
                if (!string.IsNullOrEmpty(property.Name) && o != null)
                {
                    propertyName.Add(property.Name, o.ToString());
                }
                // }
            }

            return propertyName;
        }

    }
    /// <summary>
    ///  flexigrid与后台传输的对象
    /// </summary>
    [DataContract]
    public class DataGridParam
    {
        /// 加密key
        private string _key = "fa12cad9122f4bf7905bcfa07afd3674";
        private string _sortName = "CreateTime";
        private string _sortOrder = "desc";
        private string _userId = "";
        [DataMember]
        public int Page { get; set; }
        [DataMember]
        public int RP { get; set; }
        [DataMember]
        public string SortName
        {
            get { return _sortName; }
            set { _sortName = value; }
        }
        [DataMember]
        public string SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; }
        }
        [DataMember]
        public string Query { get; set; }
        [DataMember]
        public string QType { get; set; }
        [DataMember]
        public string Cols { get; set; }
        [DataMember]
        public string UserId {
            get { return MeDes.uncMe(_userId, _key); }
            set { _userId = value; }
        }
    }

    [DataContract]
    public class Menu
    {
        [DataMember]
        public int MenuId { get; set; }
        [DataMember]
        public string Icon { get; set; }
        [DataMember]
        public string MenuName { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Remark { get; set; }
        [DataMember]
        public List<Menu> Menus { get; set; }

    }


    public class FunctionS
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelect { get; set; }
    }
}
