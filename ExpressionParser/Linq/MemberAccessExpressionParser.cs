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
            var memberName = expression.GetPropertyName('.');

            if (expression.Expression is MemberExpression)
            {
                var result = GetParser(expression.Expression).Parse(queryMapping);

                if (result is MemberAccessNode node)
                {
                    if (!queryMapping.Mappings.TryGetValue(memberName, out var sidentifier))
                        sidentifier = memberName;

                    return new MemberAccessNode(node.MemberType, sidentifier, memberName);
                }

                return result;
            }

            if (expression.Expression is ConstantExpression)
            {
                var objectMember = Expression.Convert(expression, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                var targetValue = getter();

                return new ConstantNode(expression.Type, targetValue);
            }

            if (!queryMapping.Mappings.TryGetValue(memberName, out var identifier))
                identifier = memberName;

            return new MemberAccessNode(expression.Type, identifier, memberName);
        }


    }
}
