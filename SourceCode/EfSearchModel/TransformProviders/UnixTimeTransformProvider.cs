using System;
using System.Collections.Generic;
using EfSearchModel.Model;

namespace EfSearchModel
{
    public class UnixTimeTransformProvider : ITransformProvider
    {
        public bool Match(ConditionItem item, Type type)
        {
            var elementType = TypeUtil.GetUnNullableType(type);
            return ((elementType == typeof (int) && !(item.Value is int))
                    || (elementType == typeof (long) && !(item.Value is long))
                    || (elementType == typeof (DateTime) && !(item.Value is DateTime))
                   )
                   && item.Value.ToString().Contains("-");
        }

        public IEnumerable<ConditionItem> Transform(ConditionItem item, Type type)
        {
            DateTime willTime;
            if (DateTime.TryParse(item.Value.ToString(), out willTime))
            {
                var method = item.Method;
                
                if (method == QueryMethod.LessThan || method == QueryMethod.LessThanOrEqual)
                {
                    method = QueryMethod.DateTimeLessThanOrEqual;
                    if (willTime.Hour == 0 && willTime.Minute == 0 && willTime.Second == 0)
                    {
                        willTime = willTime.AddDays(1).AddMilliseconds(-1);
                    }
                }
                object value = null;
                var elementType = TypeUtil.GetUnNullableType(type);
                if (elementType == typeof(DateTime))
                {
                    value = willTime;
                }
                else if (elementType == typeof(int))
                {
                    value = (int)UnixTime.FromDateTime(willTime);
                }
                else if (elementType == typeof(long))
                {
                    value = UnixTime.FromDateTime(willTime);
                }
                return new[] { new ConditionItem(item.Field, method, value) };
            }

            return new[] { new ConditionItem(item.Field, item.Method, Convert.ChangeType(item.Value,type)) };
        }
    }
}