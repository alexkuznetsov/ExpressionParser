using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq;

internal class BinaryExpressionParser : Parser
{
    private readonly BinaryExpression _node;

    public BinaryExpressionParser(BinaryExpression node)
    {
        this._node = node;
    }

    public override Node Parse()
    {
        var leftNode = GetParser(_node.Left).Parse();
        var rightNode = GetParser(_node.Right).Parse();

        return new BinaryNode(ParseOperation(_node.NodeType))
        {
            LeftNode = leftNode,
            RightNode = rightNode
        };
    }
}
