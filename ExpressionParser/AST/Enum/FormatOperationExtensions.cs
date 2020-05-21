using System;

namespace ExpressionParser.AST.Enum
{
    internal static class FormatOperationExtensions
    {
        internal static string AsString(this Operation operation)
        {
            return operation switch
            {
                Operation.Equal => "=",
                Operation.AndAlso => "AND",
                Operation.OrElse => "OR",
                Operation.NotEqual => "!=",
                Operation.GreaterThan => ">",
                Operation.GreaterThanOrEqual => ">=",
                Operation.LessThan => "<",
                Operation.LessThanOrEqual => "<=",
                Operation.Negate => "-",
                Operation.NegateChecked => "-",
                Operation.Not => "!",
                Operation.Like => "LIKE",
                Operation.In => "IN",
                _ => throw new NotSupportedException($"Operation {operation} not supported"),
            };
        }
    }


}
