using System.Linq.Expressions;
using ExpressionParser.AST;

namespace ExpressionParser.Linq
{
    internal class ConstantExpressionParser : Parser
    {
        private readonly ConstantExpression expression;

        public ConstantExpressionParser(ConstantExpression expression)
        {
            this.expression = expression;
        }

        public override Node Parse()
        {
            var type = expression.Type;
            return new ConstantNode(type, expression.Value);
        }
    }
}
