using System.Collections;

using ExpressionParser.AST;

namespace ExpressionParser.Format
{
    internal class MemberAccessFormatter : SqlFormatter
    {
        private readonly BinaryNode binary;

        public MemberAccessFormatter(BinaryNode binary)
        {
            this.binary = binary;
        }

        public override void Format(NodeExpression finalExpression, IQueryMapping mapping)
        {
            var me = (MemberAccessNode)binary.LeftNode;
            var ce = (ConstantNode)binary.RightNode;

            finalExpression.Append('(');

            FormatMemberExpression(me, ce, finalExpression, mapping);

            finalExpression.Append(' ');

            finalExpression.Append((ce.Value is null && !ce.ForceParameter)
                    ? (binary.Operation == Operation.Equal ? "IS NULL" : "IS NOT NULL")
                    :
                    OperationAsString(binary.Operation));

            FormatValueExpression(me, ce, finalExpression);

            finalExpression.Append(')');
        }

        private void FormatValueExpression(MemberAccessNode me, ConstantNode ce, NodeExpression finalExpression)
        {
            if (ce.Value is null && !ce.ForceParameter)
                return;
            
            finalExpression.Append(' ');

            var memberName = me.Parent != null ? $"{me.Parent.MemberName}.{me.MemberName}" : me.MemberName;
            var parameterName = memberName.Replace(".", "");
            var type = ce.ParameterType;

            if (type != typeof(string) && (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type)))
            {
                parameterName += "Collection";
            }

            if (me.Formatter != null)
            {
                finalExpression.Append(me.Formatter($"@{parameterName}"));
            }
            else
            {
                finalExpression.Append($"@{parameterName}");
            }

            finalExpression.Parameters.Add(new NodeParameter(parameterName, ce.Value));
        }

        private void FormatMemberExpression(MemberAccessNode me, ConstantNode ce, NodeExpression finalExpression, IQueryMapping mapping)
        {
            var memberName = me.Parent != null ? $"{me.Parent.MemberName}.{me.MemberName}" : me.MemberName;

            if (me.MemberName.StartsWith('@'))
            {
                var parameterName = (me.Parent != null ? $"{me.Parent.MemberName}{me.MemberName}" : me.MemberName)
                    .Replace(".", "")
                    .Replace("@", "");

                var type = ce.ParameterType;

                if (type != typeof(string) && (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type)))
                {
                    parameterName += "Collection";
                }
                finalExpression.Append('@');
                finalExpression.Append(parameterName);
            }
            else
            {
                if (!mapping.Mappings.TryGetValue(memberName, out var identifier))
                {
                    identifier = memberName;
                }

                finalExpression.Append(identifier);
            }
        }
    }
}
