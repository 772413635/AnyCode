using System;
using System.Data.Linq;

namespace LinqTODB
{
    public static class DataContextExtensions
    {
        public static ITable GetTableByName(this DataContext context, string tableName)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (tableName == null)
            {
                throw new ArgumentNullException("tableName");
            }
            return (ITable)context.GetType().GetProperty(tableName).GetValue(context, null);
            
        }
    }
}
