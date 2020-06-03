using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq
{
    internal class BinaryExpressionParser : Parser
    {
        private readonly BinaryExpression node;

        public BinaryExpressionParser(BinaryExpression node)
        {
            this.node = node;
        }

        public override Node Parse()
        {
            var leftNode = GetParser(node.Left).Parse();
            var rightNode = GetParser(node.Right).Parse();

            return new BinaryNode(ParseOperation(node.NodeType))
            {
                LeftNode = leftNode,
                RightNode = rightNode
            };
        }
    }
}
