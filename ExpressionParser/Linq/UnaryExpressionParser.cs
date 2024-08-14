using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq;

internal class UnaryExpressionParser : Parser
{
    private readonly UnaryExpression _expression;

    public UnaryExpressionParser(UnaryExpression expression)
    {
        this._expression = expression;
    }

    public override Node Parse() => GetParser(_expression.Operand).Parse();
}
