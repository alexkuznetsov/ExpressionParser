using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq
{
    internal class LikeOrNullExpressionParser : Parser
    {
        private readonly MethodCallExpression expression;

        public LikeOrNullExpressionParser(MethodCallExpression expression)
        {
            this.expression = expression;
        }

        public override Node Parse()
        {
            var memberNode = (MemberAccessNode)GetParser(expression.Arguments[0]).Parse();

            memberNode.Formatter = (s) => @$"'%' + {s} + '%'";

            var valNode = (ConstantNode)GetParser(expression.Arguments[1]).Parse();
            valNode.ForceParameter = true;

            return new BinaryNode(Operation.OrElse)
            {
                LeftNode = new BinaryNode(Operation.Like)
                {
                    LeftNode = memberNode,
                    RightNode = valNode
                },
                RightNode = new BinaryNode(Operation.Equal)
                {
                    LeftNode = new MemberAccessNode(memberNode.MemberType, $"@{memberNode.MemberName}", memberNode.Parent),
                    RightNode = new ConstantNode(memberNode.MemberType, null)
                }
            };
        }
    }
}
