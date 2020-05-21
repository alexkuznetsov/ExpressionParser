using System.Linq.Expressions;

using ExpressionParser.AST;
using ExpressionParser.AST.Enum;

namespace ExpressionParser.Linq
{
    internal class LikeOrNullExpressionParser : Parser
    {
        private readonly MethodCallExpression expression;

        public LikeOrNullExpressionParser(MethodCallExpression expression)
        {
            this.expression = expression;
        }

        public override Node Parse(IQueryMapping queryMapping)
        {
            var propertyAccess = (MemberAccessNode)GetParser(expression.Arguments[0]).Parse(queryMapping);

            propertyAccess.Formatter = (s) => @$"'%' + {s} + '%'";

            var values = (ConstantNode)GetParser(expression.Arguments[1]).Parse(queryMapping);
            values.ForceParameter = true;

            return new BinaryNode(Operation.OrElse)
            {
                LeftNode = new BinaryNode(Operation.Like)
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
