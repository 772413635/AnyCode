using System;
using System.Collections.Generic;
using EfSearchModel.Model;

namespace EfSearchModel
{
    public interface ITransformProvider
    {
        bool Match(ConditionItem item, Type type);
        IEnumerable<ConditionItem> Transform(ConditionItem item, Type type);
    }
}