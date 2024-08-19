using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq;

internal class LambdaExpressionParser : Parser
{
    private readonly LambdaExpression _node;

    public LambdaExpressionParser(LambdaExpression node) => this._node = node;

    public override Node Parse() => GetParser(_node.Body).Parse();
}
