using System;
using System.Linq.Expressions;

using ExpressionParser.AST;
using ExpressionParser.AST.Enum;
using ExpressionParser.Linq;

namespace ExpressionParser
{
    public abstract class Parser
    {
        public static Parser GetParser(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    return new ConstantExpressionParser((ConstantExpression)expression);
                case ExpressionType.Lambda:
                    return new LambdaExpressionParser((LambdaExpression)expression);
                case ExpressionType.Add:
                case ExpressionType.OrElse:
                case ExpressionType.AndAlso:
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                    return new BinaryExpressionParser((BinaryExpression)expression);
                case ExpressionType.MemberAccess:
                    return new MemberAccessExpressionParser((MemberExpression)expression);
                case ExpressionType.Convert:
                    return new UnaryExpressionParser((UnaryExpression)expression);
                case ExpressionType.Call:
                    return new MethodCallExpressionParser((MethodCallExpression)expression);
                default:
                    throw new NotImplementedException($"Parser for {expression.NodeType} not implemented");
            }
        }

        public static NodeExpression CreateResult(Node node, IQueryMapping mapping)
        {
            var finalExpression = new NodeExpression();
            node.Visit(finalExpression, mapping);

            return finalExpression;
        }

        public abstract Node Parse();

        protected Operation ParseOperation(ExpressionType expressionType)
        {
            return expressionType switch
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
    }
}
