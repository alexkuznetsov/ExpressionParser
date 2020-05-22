using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq
{
    internal class MemberAccessExpressionParser : Parser
    {
        private readonly MemberExpression expression;

        public MemberAccessExpressionParser(MemberExpression expression)
        {
            this.expression = expression;
        }

        public override Node Parse()
        {
            var currentNode = CreateMemberAccessNode();

            if (expression.Expression.NodeType != ExpressionType.Parameter)
            {
                var subNode = GetParser(expression.Expression).Parse();

                if (subNode is MemberAccessNode sub)
                {
                    return new MemberAccessNode(currentNode.MemberType, currentNode.MemberName, sub)
                    {
                        Formatter = currentNode.Formatter
                    };
                }
                else if (subNode is ConstantNode)
                {
                    var objectMember = Expression.Convert(expression, typeof(object));
                    var getterLambda = Expression.Lambda<System.Func<object>>(objectMember);
                    var getter = getterLambda.Compile();
                    var targetValue = getter();

                    return new ConstantNode(expression.Type, targetValue);
                }
            }

            return currentNode;
        }


        private MemberAccessNode CreateMemberAccessNode()
        {
            return new MemberAccessNode(expression.Type, expression.Member.Name);
        }

    }
}
