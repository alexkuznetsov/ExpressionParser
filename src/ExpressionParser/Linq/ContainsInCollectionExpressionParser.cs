using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq;

internal class ContainsInCollectionExpressionParser : Parser
{
    private readonly MethodCallExpression _expression;

    public ContainsInCollectionExpressionParser(MethodCallExpression expression)
    {
        this._expression = expression;
    }

    public override Node Parse()
    {
        var valuesSet = GetParser(_expression.Arguments[0]);
        var memberAccess = GetParser(_expression.Arguments[1]);

        var valueSetResult = valuesSet.Parse();
        var memberAccessResult = memberAccess.Parse();

        return new BinaryNode(Operation.In)
        {
            LeftNode = memberAccessResult,
            RightNode = valueSetResult
        };
    }
}
