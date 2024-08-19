using System;

namespace ExpressionParser.AST;

/// <summary>
/// Const
/// </summary>
public class ConstantNode : Node
{
    
    /// <summary>
    /// Const val
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Interpret const as a parameter
    /// </summary>
    public bool ForceParameter { get; set; }

    /// <summary>
    /// Const value type
    /// </summary>
    public Type ParameterType { get; }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public ConstantNode(Type type, object value)
    {
        ParameterType = type;
        Value = value;
    }
}
