using System;

namespace ExpressionParser.AST
{
    public class MethodCallNode : Node
    {
        public string MemberName { get; internal set; }
        public Func<string, string> Formatter { get; internal set; }
    }
}
