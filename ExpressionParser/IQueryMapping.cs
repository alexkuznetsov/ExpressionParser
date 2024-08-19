using System.Collections.Generic;

namespace ExpressionParser;

public interface IQueryMapping
{
    string TableAlias { get; }

    IReadOnlyDictionary<string, string> Mappings { get; }
}
