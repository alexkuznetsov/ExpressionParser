using System;
using System.Linq.Expressions;

using ExpressionParser.AST;
using ExpressionParser.AST.Enum;

namespace ExpressionParser.Linq
{
    internal class ContainsOrNullExpressionParser : Parser
    {
        private readonly MethodCallExpression expression;

        public ContainsOrNullExpressionParser(MethodCallExpression expression)
        {
            this.expression = expression;
        }

        public override Node Parse(IQueryMapping queryMapping)
        {
            var arg1 = GetParser(expression.Arguments[0]).Parse(queryMapping);
            var arg2 = GetParser(expression.Arguments[1]).Parse(queryMapping);

            if (arg1 is ConstantNode cn1 && arg2 is MemberAccessNode man1)
            {
                return ParseContainsInCollection(cn1, man1);
            }
            else if (arg1 is MemberAccessNode man2 && arg2 is ConstantNode cn2)
            {
                return ParseStringContains(man2, cn2);
            }

            throw new NotSupportedException($"ContainsOrNullExpressionParser: {expression} not supported");
        }

        private Node ParseStringContains(
              MemberAccessNode memberAccessResult
            , ConstantNode valueSetResult)
        {
            //var a1 = GetParser(expression.Arguments[0]);
            //var a2 = GetParser(expression.Arguments[1]);

            //var memberAccessResult = (MemberAccessNode)a1.Parse(queryMapping);
            //var valueSetResult = (ConstantNode)a2.Parse(queryMapping);

            memberAccessResult.Formatter = (s) => "'%' + " + s + " + '%'";
            valueSetResult.ForceParameter = true;

            return new BinaryNode(Operation.OrElse) /* property like @val or @val is null */
            {
                LeftNode = new BinaryNode(Operation.Like)
                {
                    LeftNode = memberAccessResult,
                    RightNode = valueSetResult
                },
                RightNode = new BinaryNode(Operation.Equal)
                {
                    LeftNode = new MemberAccessNode(valueSetResult.ParameterType, $"@{memberAccessResult.MemberName}", memberAccessResult.MemberName),
                    RightNode = new ConstantNode(valueSetResult.ParameterType, null)
                }
            };
        }

        private Node ParseContainsInCollection( 
              ConstantNode valueSetResult
            , MemberAccessNode memberAccessResult)
        {
            //var valuesSet = GetParser(expression.Arguments[0]);
            //var memberAccess = GetParser(expression.Arguments[1]);

            //var valueSetResult = (ConstantNode)valuesSet.Parse(queryMapping);
            //var memberAccessResult = (MemberAccessNode)memberAccess.Parse(queryMapping);

            return new BinaryNode(Operation.OrElse) /* property in @collection or @collection is null */
            {
                LeftNode = new BinaryNode(Operation.In)
                {
                    LeftNode = memberAccessResult,
                    RightNode = valueSetResult
                },
                RightNode = new BinaryNode(Operation.Equal)
                {
                    LeftNode = new MemberAccessNode(valueSetResult.ParameterType, $"@{memberAccessResult.MemberName}", memberAccessResult.MemberName),
                    RightNode = new ConstantNode(valueSetResult.ParameterType, null)
                }
            };
        }
    }
}
