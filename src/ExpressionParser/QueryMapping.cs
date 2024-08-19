using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionParser;

public class QueryMapping<TModel> : IQueryMapping
{
    private readonly Dictionary<string, string> _mapping = [];

    protected QueryMapping(string tableAlias)
    {
        TableAlias = tableAlias;
    }

    protected void AutoMapAllProperties()
    {
        foreach (var p in typeof(TModel).GetProperties())
        {
            AddMapping(p.Name);
        }
    }

    protected void Map<TKey>(Expression<Func<TModel, TKey>> propertyExpression, string sqlAccessExpression, string tableAliasOverride = default)
    {
        var name = propertyExpression.GetPropertyName();
        AddMapping(name, sqlAccessExpression, tableAliasOverride);
    }

    protected void MapToSameWithUnderscores<TKey>(Expression<Func<TModel, TKey>> propertyExpression, string tableAliasOverride = default)
    {
        var name = propertyExpression.GetPropertyName();
        AddMapping(name, tableAliasOverride);
    }

    protected void AddMapping(string modelPropery, string tableAliasOverride = default)
    {
        var sqlAccessExpression = ToName(modelPropery).ToLowerInvariant();

        AddMapping(modelPropery, sqlAccessExpression, tableAliasOverride);
    }

    protected void AddMapping(string modelPropery, string sqlAccessExpression, string tableAliasOverride = default)
    {
        var finalTableAlias = (tableAliasOverride != default ? tableAliasOverride : TableAlias);
        var mapTo = $"{finalTableAlias}.{sqlAccessExpression}";

        _mapping.Add(modelPropery, mapTo);
    }

    private string ToName(string input)
    {
        return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())); ;
    }
    public IReadOnlyDictionary<string, string> Mappings => _mapping;

    public string TableAlias { get; }
}
