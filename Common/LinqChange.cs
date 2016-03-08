using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Reflection;

namespace Common
{
    public class LinqChange
    {
        /// <summary>
        /// 拷贝对象的属性值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="tSource">源对象</param>
        /// <param name="tDestination">设置属性值的对象</param>
        public static void CopyObjectProperty<T>(T tDestination,dynamic tSource) where T : class
        {
            //获得所有property的信息
            PropertyInfo[] properties = tSource.GetType().GetProperties();
            foreach (PropertyInfo p in properties)
            {
                tDestination.GetType().GetProperty(p.Name).SetValue(tDestination, p.GetValue(tSource, null), null);//设置tDestination的属性值       
            }
        }

        //表中有数据或无数据时使用,可排除DATASET不支持System.Nullable错误
        public static DataTable ConvertToDataSet<T>(IList<T> list)
        {
            if (list == null || list.Count <= 0)
            //return null;
            {
                DataTable result = new DataTable();
                if (list.Count > 0)
                {
                    PropertyInfo[] propertys = list[0].GetType().GetProperties();
                    foreach (PropertyInfo pi in propertys)
                    {
                        //if (!(pi.Name.GetType() is System.Nullable))
                        //if (pi!=null)
                        {
                            //pi = (PropertyInfo)temp;   
                            result.Columns.Add(pi.Name, pi.PropertyType);
                        }

                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        ArrayList tempList = new ArrayList();
                        foreach (PropertyInfo pi in propertys)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        object[] array = tempList.ToArray();
                        result.LoadDataRow(array, true);
                    }
                }
                return result;
            }
            DataSet ds = new DataSet();
            DataTable dt = new DataTable(typeof(T).Name);
            DataColumn column;
            DataRow row;

            PropertyInfo[] myPropertyInfo =
                typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (T t in list)
            {
                if (t == null) continue;
                row = dt.NewRow();

                for (int i = 0, j = myPropertyInfo.Length; i < j; i++)
                {
                    PropertyInfo pi = myPropertyInfo[i];
                    String name = pi.Name;

                    if (dt.Columns[name] == null)
                    {
                        if (pi.PropertyType.UnderlyingSystemType.ToString() == "System.Nullable`1[System.Int32]")
                        {
                            column = new DataColumn(name, typeof(Int32));
                            dt.Columns.Add(column);
                            //row[name] = pi.GetValue(t, new object[] {i});//PropertyInfo.GetValue(object,object[])
                            if (pi.GetValue(t, null) != null)
                                row[name] = pi.GetValue(t, null);
                            else
                                row[name] = DBNull.Value;
                        }
                        else
                        {
                            column = new DataColumn(name, pi.PropertyType);
                            dt.Columns.Add(column);
                            row[name] = pi.GetValue(t, null);
                        }
                    }
                    else
                    {
                        row[name] = pi.GetValue(t, null);
                    }
                }
                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);
            return ds.Tables[0];
        }

        public static void Detatch<TEntity>(TEntity entity)
        {
            Type t = entity.GetType();
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                string name = property.Name;
                if (property.PropertyType.IsGenericType &&
                property.PropertyType.GetGenericTypeDefinition() == typeof(EntitySet<>))
                {
                    property.SetValue(entity, null, null);
                }
            }
            FieldInfo[] fields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                string name = field.Name;
                if (field.FieldType.IsGenericType &&
                field.FieldType.GetGenericTypeDefinition() == typeof(EntityRef<>))
                {
                    field.SetValue(entity, null);
                }
            }
            EventInfo eventPropertyChanged = t.GetEvent("PropertyChanged");
            EventInfo eventPropertyChanging = t.GetEvent("PropertyChanging");
            if (eventPropertyChanged != null)
            {
                eventPropertyChanged.RemoveEventHandler(entity, null);
            }
            if (eventPropertyChanging != null)
            {
                eventPropertyChanging.RemoveEventHandler(entity, null);
            }
        }
    }
}
