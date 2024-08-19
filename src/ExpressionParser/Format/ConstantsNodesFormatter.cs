
using ExpressionParser.AST;

namespace ExpressionParser.Format;

internal class ConstantsNodesFormatter : SqlFormatter
{
    private readonly BinaryNode _binary;

    public ConstantsNodesFormatter(BinaryNode binary)
    {
        this._binary = binary;
    }

    public override void Format(NodeExpression finalExpression, IQueryMapping mapping)
    {
        ConstantNode c1 = (ConstantNode)_binary.LeftNode;
        ConstantNode c2 = (ConstantNode)_binary.RightNode;

        finalExpression.Append('(');
        finalExpression.Append(c1.Value);
        finalExpression.Append(' ');
        finalExpression.Append(OperationAsString(_binary.Operation));
        finalExpression.Append(' ');
        finalExpression.Append(c2.Value);
        finalExpression.Append(')');
    }
}
