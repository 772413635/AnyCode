using System;
using System.Collections.Generic;
using System.Linq;
using EfSearchModel.Model;

namespace EfSearchModel
{
    public class LikeTransformProvider : ITransformProvider
    {
        public bool Match(ConditionItem item, Type type)
        {
            return item.Method == QueryMethod.Like;
        }

        public IEnumerable<ConditionItem> Transform(ConditionItem item, Type type)
        {
            var str = item.Value.ToString();
            var keyWords = str.Split('*');
            if (keyWords.Length == 1)
            {
                return new[] { new ConditionItem(item.Field, QueryMethod.Equal, item.Value) };
            }
            var list = new List<ConditionItem>();
            if (!string.IsNullOrEmpty(keyWords.First()))
                list.Add(new ConditionItem(item.Field, QueryMethod.StartsWith, keyWords.First()));
            if (!string.IsNullOrEmpty(keyWords.Last()))
                list.Add(new ConditionItem(item.Field, QueryMethod.EndsWith, keyWords.Last()));
            for (int i = 1; i < keyWords.Length - 1; i++)
            {
                if (!string.IsNullOrEmpty(keyWords[i]))
                    list.Add(new ConditionItem(item.Field, QueryMethod.Contains, keyWords[i]));
            }
            return list;
        }
    }
}