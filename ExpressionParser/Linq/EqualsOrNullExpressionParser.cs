using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq
{
    internal class EqualsOrNullExpressionParser : Parser
    {
        private readonly MethodCallExpression expression;

        public EqualsOrNullExpressionParser(MethodCallExpression expression)
        {
            this.expression = expression;
        }

        public override Node Parse()
        {
            var memberNode = (MemberAccessNode)GetParser(expression.Arguments[0]).Parse();
            var valNode = (ConstantNode)GetParser(expression.Arguments[1]).Parse();
            valNode.ForceParameter = true;

            return new BinaryNode(Operation.OrElse)
            {
                LeftNode = new BinaryNode(Operation.Equal)
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
