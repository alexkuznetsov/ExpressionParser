using System;

namespace ExpressionParser.AST
{
    public class MemberAccessNode : Node
    {
        public string Identifier { get; }

        public string MemberName { get; }

        public Func<string, string> Formatter { get; set; }

        public Type MemberType { get; }

        public MemberAccessNode(Type memberType, string identifier, string memberName)
        {
            MemberType = memberType;
            Identifier = identifier;
            MemberName = memberName;
        }

        public override void Visit(NodeExpression finalExpression) => throw new InvalidOperationException();
    }


}
