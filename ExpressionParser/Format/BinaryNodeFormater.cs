using ExpressionParser.AST;

namespace ExpressionParser.Format;

internal class BinaryNodeFormater : SqlFormatter
{
    private readonly BinaryNode _binary;

    public BinaryNodeFormater(BinaryNode binary)
    {
        this._binary = binary;
    }

    public override void Format(NodeExpression finalExpression, IQueryMapping mapping)
    {
        finalExpression.Append('(');

        GetForNode(_binary.LeftNode).Format(finalExpression, mapping);

        finalExpression.Append(' ');
        finalExpression.Append(OperationAsString(_binary.Operation));
        finalExpression.Append(' ');

        GetForNode(_binary.RightNode).Format(finalExpression, mapping);

        finalExpression.Append(')');
    }
}