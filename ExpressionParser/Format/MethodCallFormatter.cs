using System.Collections;

using ExpressionParser.AST;

namespace ExpressionParser.Format
{
    internal class MethodCallFormatter : SqlFormatter
    {
        private readonly BinaryNode binary;

        public MethodCallFormatter(BinaryNode binary)
        {
            this.binary = binary;
        }

        public override void Format(NodeExpression finalExpression, IQueryMapping mapping)
        {
            MethodCallNode methodCallNode = (MethodCallNode)binary.LeftNode;
            ConstantNode methodCallArg = (ConstantNode)binary.RightNode;

            if (!mapping.Mappings.TryGetValue(methodCallNode.MemberName, out var identifier))
            {
                identifier = methodCallNode.MemberName;
            }

            finalExpression.Append('(');
            finalExpression.Append(identifier);

            finalExpression.Append(' ');
            finalExpression.Append(OperationAsString(binary.Operation));
            finalExpression.Append(' ');

            var parameterName = methodCallNode.MemberName.Replace(".", "");
            var type = methodCallArg.ParameterType;


            if (type != typeof(string) && (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type)))
            {
                parameterName += "Collection";
            }

            finalExpression.Append(methodCallNode.Formatter($"@{parameterName}"));

            finalExpression.Parameters.Add(new NodeParameter(parameterName, methodCallArg.Value));
            finalExpression.Append(')');
        }
    }
}
