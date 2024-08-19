using System;
using System.Linq.Expressions;

using ExpressionParser.AST;
using ExpressionParser.Format;
using ExpressionParser.Linq;

namespace ExpressionParser;

public abstract class Parser
{
    public static Parser GetParser(Expression expression)
    {
        return expression.NodeType switch
        {
            ExpressionType.Constant => new ConstantExpressionParser((ConstantExpression)expression),
            ExpressionType.Lambda => new LambdaExpressionParser((LambdaExpression)expression),
            ExpressionType.Add or ExpressionType.OrElse or ExpressionType.AndAlso or ExpressionType.Equal or ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual or ExpressionType.LessThan or ExpressionType.LessThanOrEqual or ExpressionType.NotEqual => new BinaryExpressionParser((BinaryExpression)expression),
            ExpressionType.MemberAccess => new MemberAccessExpressionParser((MemberExpression)expression),
            ExpressionType.Convert => new UnaryExpressionParser((UnaryExpression)expression),
            ExpressionType.Call => new MethodCallExpressionParser((MethodCallExpression)expression),
            _ => throw new NotImplementedException($"Parser for {expression.NodeType} not implemented"),
        };
    }

    public static NodeExpression CreateResult(Node node, IQueryMapping mapping)
    {
        var finalExpression = new NodeExpression();

        SqlFormatter.GetForNode(node).Format(finalExpression, mapping);

        return finalExpression;
    }

    public abstract Node Parse();

    protected static Operation ParseOperation(ExpressionType expressionType) => expressionType switch
    {
        ExpressionType.Equal => Operation.Equal /*"="*/,
        ExpressionType.AndAlso => Operation.AndAlso/*"and"*/,
        ExpressionType.OrElse => Operation.OrElse/*"or"*/,
        ExpressionType.NotEqual => Operation.NotEqual/*"!="*/,
        ExpressionType.GreaterThan => Operation.GreaterThan/*">"*/,
        ExpressionType.GreaterThanOrEqual => Operation.GreaterThanOrEqual/* ">="*/,
        ExpressionType.LessThan => Operation.LessThan /*"<"*/,
        ExpressionType.LessThanOrEqual => Operation.LessThanOrEqual/*"<="*/,
        ExpressionType.Negate => Operation.Negate/*"-"*/,
        ExpressionType.NegateChecked => Operation.NegateChecked/*"-"*/,
        ExpressionType.Not => Operation.Not/*"!"*/,

        _ => throw new NotImplementedException($"Operation {expressionType} not supported"),
    };
}
