using System.Collections.Generic;

namespace ExpressionParser;

/// <summary>
/// Query mapping (name to sql name)
/// </summary>
public interface IQueryMapping
{
    /// <summary>
    /// Main table short name
    /// </summary>
    string TableAlias { get; }

    /// <summary>
    /// Related table mappings
    /// </summary>
    IReadOnlyDictionary<string, string> Mappings { get; }
}
