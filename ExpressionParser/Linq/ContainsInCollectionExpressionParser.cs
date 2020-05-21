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

        public override Node Parse(IQueryMapping queryMapping)
        {
            var valuesSet = GetParser(expression.Arguments[0]);
            var memberAccess = GetParser(expression.Arguments[1]);

            var valueSetResult = valuesSet.Parse(queryMapping);
            var memberAccessResult = memberAccess.Parse(queryMapping);

            return new BinaryNode(Operation.In)
            {
                LeftNode = memberAccessResult,
                RightNode = valueSetResult
            };
        }
    }
}
