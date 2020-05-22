using System.Linq.Expressions;

using ExpressionParser.AST;
using ExpressionParser.AST.Enum;

namespace ExpressionParser.Linq
{
    internal class ContainsInCollectionExpressionParser : Parser
    {
        private readonly MethodCallExpression expression;

        public ContainsInCollectionExpressionParser(MethodCallExpression expression)
        {
            this.expression = expression;
        }

        public override Node Parse()
        {
            var valuesSet = GetParser(expression.Arguments[0]);
            var memberAccess = GetParser(expression.Arguments[1]);

            var valueSetResult = valuesSet.Parse();
            var memberAccessResult = memberAccess.Parse();

            return new BinaryNode(Operation.In)
            {
                LeftNode = memberAccessResult,
                RightNode = valueSetResult
            };
        }
    }
}
