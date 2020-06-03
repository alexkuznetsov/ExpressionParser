using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq
{
    internal class MethodCallExpressionParser : Parser
    {
        private readonly MethodCallExpression expression;

        public MethodCallExpressionParser(MethodCallExpression expression)
        {
            this.expression = expression;
        }

        public override Node Parse()
        {
            var functionParser = MethodCallParsers.DetectWhoCanAccept(expression);

            return functionParser.Parse();
        }
    }
}
