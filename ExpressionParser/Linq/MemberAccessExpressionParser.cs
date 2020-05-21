using System;
using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq
{
    internal class MemberAccessExpressionParser : Parser
    {
        private readonly MemberExpression expression;

        public MemberAccessExpressionParser(MemberExpression expression)
        {
            this.expression = expression;
        }

        public override Node Parse(IQueryMapping queryMapping)
        {
            if (expression.Expression != null && expression.Expression.NodeType != ExpressionType.Parameter)
            {
                if (expression.Expression.NodeType == ExpressionType.Constant)
                {
                    var objectMember = Expression.Convert(expression, typeof(object));
                    var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                    var getter = getterLambda.Compile();
                    var targetValue = getter();

                    return new ConstantNode(expression.Type, targetValue);
                }
            }

            if (!queryMapping.Mappings.TryGetValue(expression.Member.Name, out var replacement))
                replacement = expression.Member.Name;

            return new MemberAccessNode(expression.Type, replacement, expression.Member.Name);
        }
    }
}
