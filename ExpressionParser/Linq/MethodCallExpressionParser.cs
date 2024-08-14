using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq;

internal class MethodCallExpressionParser : Parser
{
    private readonly MethodCallExpression _expression;

    public MethodCallExpressionParser(MethodCallExpression expression)
    {
        this._expression = expression;
    }

    public override Node Parse()
    {
        var functionParser = MethodCallParsers.DetectWhoCanAccept(_expression);

        return functionParser.Parse();
    }
}
