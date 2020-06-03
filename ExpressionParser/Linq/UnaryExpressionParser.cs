using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq
{
    internal class UnaryExpressionParser : Parser
    {
        private readonly UnaryExpression expression;

        public UnaryExpressionParser(UnaryExpression expression)
        {
            this.expression = expression;
        }

        public override Node Parse() => GetParser(expression.Operand).Parse();
    }
}
