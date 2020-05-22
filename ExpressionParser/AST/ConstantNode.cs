using System;

namespace ExpressionParser.AST
{
    public class ConstantNode : Node
    {
        public object Value { get; }

        public bool ForceParameter { get; set; }

        public Type ParameterType { get; }

        public ConstantNode(Type type, object value)
        {
            ParameterType = type;
            Value = value;
        }

        public override void Visit(NodeExpression finalExpression, IQueryMapping mapping) => throw new InvalidOperationException();
    }


}
