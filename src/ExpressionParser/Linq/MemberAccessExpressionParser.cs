using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq;

internal class MemberAccessExpressionParser : Parser
{
    private readonly MemberExpression _expression;

    public MemberAccessExpressionParser(MemberExpression expression)
    {
        this._expression = expression;
    }

    public override Node Parse()
    {
        var currentNode = CreateMemberAccessNode();

        if (_expression.Expression.NodeType != ExpressionType.Parameter)
        {
            var subNode = GetParser(_expression.Expression).Parse();

            if (subNode is MemberAccessNode sub)
            {
                return new MemberAccessNode(currentNode.MemberType, currentNode.MemberName, sub)
                {
                    Formatter = currentNode.Formatter
                };
            }
            else if (subNode is ConstantNode)
            {
                var objectMember = Expression.Convert(_expression, typeof(object));
                var getterLambda = Expression.Lambda<System.Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                var targetValue = getter();

                return new ConstantNode(_expression.Type, targetValue);
            }
        }

        return currentNode;
    }


    private MemberAccessNode CreateMemberAccessNode()
    {
        return new MemberAccessNode(_expression.Type, _expression.Member.Name);
    }

}
