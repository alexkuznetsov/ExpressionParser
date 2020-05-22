using System;

namespace ExpressionParser.AST
{
    public class MemberAccessNode : Node
    {
        public string MemberName { get; }
        public MemberAccessNode Parent { get; }
        public Func<string, string> Formatter { get; set; }

        public Type MemberType { get; }

        public MemberAccessNode(Type memberType, string memberName)
            : this(memberType, memberName, null)
        {

        }

        public MemberAccessNode(Type memberType, string memberName, MemberAccessNode parent)
        {
            MemberType = memberType;
            MemberName = memberName;
            Parent = parent;
        }

        public override void Visit(NodeExpression finalExpression, IQueryMapping mapping) => throw new InvalidOperationException();
    }


}
