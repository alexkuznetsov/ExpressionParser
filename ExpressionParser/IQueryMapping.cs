using System.Collections.Generic;

namespace ExpressionParser
{
    public interface IQueryMapping
    {
        IReadOnlyDictionary<string, string> Mappings { get; }
    }
}
