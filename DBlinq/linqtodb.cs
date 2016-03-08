// ==============================================================================
// Linq to DB 操作类
// ==============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Linq;

namespace DBlinq
{
    public class LinqToDB:GodBuildDB
    {
        DataContext _dc;

        #region 构造函数开始
        public LinqToDB()
        {
            _dc = new GodBuildDB();
            // System.Data.Linq.DataContext _dcc = new DataContext("Data Source=58.211.88.98;Initial Catalog=CCmanager;User ID=sa;Password=allismine0");
            // System.Reflection.PropertyInfo[] ss = _dc.GetType().GetProperties();
            // System.Reflection.PropertyInfo[] ssc = _dcc.GetType().GetProperties();
            //foreach (System.Reflection.PropertyInfo pi in _dc.GetType().GetProperties())
            //{
            //    string typeName = pi.PropertyType.ToString();
            //    if (typeName.Contains("System.Data.Linq.Table"))//判断是否为数据库中的表
            //    {
            //        string tableName = typeName.Substring(typeName.IndexOf('['));
            //        tableName = tableName.Substring(1, tableName.Length - 2);//获得与数据库中表相对应的Linq类
            //        foreach (System.Reflection.PropertyInfo pro in Type.GetType(tableName).GetProperties())
            //        {
            //            int cc = 1;
            //            //获取表中的各个字段
            //        }
            //    }
            //}
        }
        public LinqToDB(IDbConnection connection)
        {
            _dc = new DataContext(connection);
        }
        public LinqToDB(DataContext dc)
        {
            _dc = dc;
        }
        #endregion 构造函数结束

        public IEnumerable<TEntity> Query<TEntity>(Func<TEntity, bool> predicate) where TEntity : class
        {
            return GetTable<TEntity>().Where<TEntity>(predicate).AsEnumerable();
        }

        public Table<TEntity> GetTable<TEntity>() where TEntity : class
        {
            return _dc.GetTable<TEntity>();
        }

        public ITable GetTable(Type type)
        {
            return _dc.GetTable(type);
        }

        public void SubmitChanges()
        {
            try
            {
                _dc.SubmitChanges();
            }
            catch
            {
                foreach (ObjectChangeConflict occ in _dc.ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.KeepCurrentValues);
                    occ.Resolve(RefreshMode.OverwriteCurrentValues);
                    occ.Resolve(RefreshMode.KeepChanges);
                }
                _dc.SubmitChanges();
            }
        }

        #region 封装Table方法开始
        public void Attach<TEntity>(TEntity entity) where TEntity : class
        {
            GetTable<TEntity>().Attach(entity);
        }

        public void Attach<TEntity>(TEntity entity, bool asModified) where TEntity : class
        {
            GetTable<TEntity>().Attach(entity, asModified);
        }

        public void Attach<TEntity>(TEntity entity, TEntity original) where TEntity : class
        {
            GetTable<TEntity>().Attach(entity, original);
        }

        public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities) where TSubEntity : class
        {
            GetTable<TSubEntity>().AttachAll<TSubEntity>(entities);
        }

        public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities, bool asModified) where TSubEntity : class
        {
            GetTable<TSubEntity>().AttachAll<TSubEntity>(entities, asModified);
        }

        public void DeleteAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities) where TSubEntity : class
        {
            GetTable<TSubEntity>().DeleteAllOnSubmit<TSubEntity>(entities);
        }

        public void DeleteAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities, Type type) where TSubEntity : class
        {
            GetTable(type).DeleteAllOnSubmit(entities);
        }

        public void DeleteAllOnSubmit(ITable entities, Type type)
        {
            GetTable(type).DeleteAllOnSubmit(entities);
        }

        public void DeleteOnSubmit<TEntity>(TEntity entity) where TEntity : class
        {
            GetTable<TEntity>().DeleteOnSubmit(entity);
        }

        public IEnumerator<TEntity> GetEnumerator<TEntity>() where TEntity : class
        {
            return GetTable<TEntity>().AsEnumerable<TEntity>().GetEnumerator();
        }

        public ModifiedMemberInfo[] GetModifiedMembers<TEntity>(TEntity entity) where TEntity : class
        {
            return GetTable<TEntity>().GetModifiedMembers(entity);
        }

        public IBindingList GetNewBindingList<TEntity>() where TEntity : class
        {
            return GetTable<TEntity>().GetNewBindingList();
        }

        public TEntity GetOriginalEntityState<TEntity>(TEntity entity) where TEntity : class
        {
            return GetTable<TEntity>().GetOriginalEntityState(entity);
        }

        public void InsertAllOnSubmit<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            GetTable<TEntity>().InsertAllOnSubmit<TEntity>(entities);
        }

        public void InsertAllOnSubmit<TEntity>(IEnumerable<TEntity> entities,Type type) where TEntity : class
        {
            GetTable(type).InsertAllOnSubmit(entities);
        }

        public void InsertAllOnSubmit(IList entities, Type type)
        {
            GetTable(type).InsertAllOnSubmit(entities);
        }

        public void InsertOnSubmit<TEntity>(TEntity entity) where TEntity : class
        {
            GetTable<TEntity>().InsertOnSubmit(entity);
        }

        public override string ToString()
        {
            return "Linq2Db Ojbect!";
        }
        #endregion 封装Table方法结束
    }

    public class LinqToDB<TEntity> where TEntity : class
    {
        DataContext _dc;

        #region 构造函数开始
        public LinqToDB()
        {
            //_dc = new CSIHRONEDataContext();
        }
        public LinqToDB(IDbConnection connection)
        {
            _dc = new DataContext(connection);
        }
        #endregion 构造函数结束

        public IEnumerable<TEntity> Query(Func<TEntity, bool> predicate)
        {
            return _dc.GetTable<TEntity>().Where(predicate).AsEnumerable();
        }

        public Table<TEntity> GetTable()
        {
            return _dc.GetTable<TEntity>();
        }

        public void SubmitChanges()
        {
            try
            {
                _dc.SubmitChanges();
            }
            catch
            {
                foreach (ObjectChangeConflict occ in _dc.ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.KeepCurrentValues);
                    occ.Resolve(RefreshMode.OverwriteCurrentValues);
                    occ.Resolve(RefreshMode.KeepChanges);
                }
                _dc.SubmitChanges();
            }
        }

        #region 封装Table方法开始
        public void Attach(TEntity entity)
        {
            GetTable().Attach(entity);
        }

        public void Attach(TEntity entity, bool asModified)
        {
            GetTable().Attach(entity, asModified);
        }

        public void Attach(TEntity entity, TEntity original)
        {
            GetTable().Attach(entity, original);
        }

        public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities) where TSubEntity : TEntity
        {
            GetTable().AttachAll(entities);
        }

        public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities, bool asModified) where TSubEntity : TEntity
        {
            GetTable().AttachAll(entities, asModified);
        }

        public void DeleteAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities) where TSubEntity : TEntity
        {
            GetTable().DeleteAllOnSubmit(entities);
        }

        public void DeleteOnSubmit(TEntity entity)
        {
            GetTable().DeleteOnSubmit(entity);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return GetTable().AsEnumerable().GetEnumerator();
        }

        public ModifiedMemberInfo[] GetModifiedMembers(TEntity entity)
        {
            return GetTable().GetModifiedMembers(entity);
        }

        public IBindingList GetNewBindingList()
        {
            return GetTable().GetNewBindingList();
        }

        public TEntity GetOriginalEntityState(TEntity entity)
        {
            return GetTable().GetOriginalEntityState(entity);
        }

        public void InsertAllOnSubmit(IEnumerable<TEntity> entities)
        {
            GetTable().InsertAllOnSubmit(entities);
        }

        public void InsertOnSubmit(TEntity entity)
        {
            GetTable().InsertOnSubmit(entity);
        }

        public override string ToString()
        {
            return "LinqToDB Ojbect!";
        }
        #endregion 封装Table方法结束
    }
}
