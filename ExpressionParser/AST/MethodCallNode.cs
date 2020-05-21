using System;

namespace ExpressionParser.AST
{
    public class MethodCallNode : Node
    {
        public string Identifier { get; internal set; }
        public string MemberName { get; internal set; }
        public Func<string, string> Formatter { get; internal set; }

        public override void Visit(NodeExpression finalExpression) => throw new InvalidOperationException();
    }


}
