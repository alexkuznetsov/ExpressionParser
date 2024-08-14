using System.Linq.Expressions;
using ExpressionParser.AST;

namespace ExpressionParser.Linq;

internal class ConstantExpressionParser : Parser
{
    private readonly ConstantExpression _expression;

    public ConstantExpressionParser(ConstantExpression expression)
    {
        this._expression = expression;
    }

    public override Node Parse()
    {
        var type = _expression.Type;
        return new ConstantNode(type, _expression.Value);
    }
}
