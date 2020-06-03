using ExpressionParser.AST;

namespace ExpressionParser.Format
{
    internal class BinaryNodeFormater : SqlFormatter
    {
        private readonly BinaryNode binary;

        public BinaryNodeFormater(BinaryNode binary)
        {
            this.binary = binary;
        }

        public override void Format(NodeExpression finalExpression, IQueryMapping mapping)
        {
            finalExpression.Append('(');

            GetForNode(binary.LeftNode).Format(finalExpression, mapping);

            finalExpression.Append(' ');
            finalExpression.Append(OperationAsString(binary.Operation));
            finalExpression.Append(' ');

            GetForNode(binary.RightNode).Format(finalExpression, mapping);

            finalExpression.Append(')');
        }
    }
}