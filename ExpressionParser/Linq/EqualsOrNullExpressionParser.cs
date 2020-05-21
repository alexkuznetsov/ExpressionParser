using System.Linq.Expressions;

using ExpressionParser.AST;
using ExpressionParser.AST.Enum;

namespace ExpressionParser.Linq
{
    internal class EqualsOrNullExpressionParser : Parser
    {
        private readonly MethodCallExpression expression;

        public EqualsOrNullExpressionParser(MethodCallExpression expression)
        {
            this.expression = expression;
        }

        public override Node Parse(IQueryMapping queryMapping)
        {
            var propertyAccess = (MemberAccessNode)GetParser(expression.Arguments[0]).Parse(queryMapping);
            var values = (ConstantNode)GetParser(expression.Arguments[1]).Parse(queryMapping);
            values.ForceParameter = true;

            return new BinaryNode(Operation.OrElse)
            {
                LeftNode = new BinaryNode(Operation.Equal)
                {
                    LeftNode = propertyAccess,
                    RightNode = values
                },
                RightNode = new BinaryNode(Operation.Equal)
                {
                    LeftNode = new MemberAccessNode(propertyAccess.MemberType, $"@{propertyAccess.MemberName}", propertyAccess.MemberName),
                    RightNode = new ConstantNode(propertyAccess.MemberType, null)
                }
            };
        }
    }
}
