
using ExpressionParser.AST;

namespace ExpressionParser.Format
{
    internal class ConstantsNodesFormatter : SqlFormatter
    {
        private readonly BinaryNode binary;

        public ConstantsNodesFormatter(BinaryNode binary)
        {
            this.binary = binary;
        }

        public override void Format(NodeExpression finalExpression, IQueryMapping mapping)
        {
            ConstantNode c1 = (ConstantNode)binary.LeftNode;
            ConstantNode c2 = (ConstantNode)binary.RightNode;

            finalExpression.Append('(');
            finalExpression.Append(c1.Value);
            finalExpression.Append(' ');
            finalExpression.Append(OperationAsString(binary.Operation));
            finalExpression.Append(' ');
            finalExpression.Append(c2.Value);
            finalExpression.Append(')');
        }
    }
}
