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
            var arg1 = expression.Arguments[0];
            var arg2 = expression.Arguments[1];

            var isArg1MemberExpression = arg1 is MemberExpression;
            var isArg2MemberExpression = arg2 is MemberExpression;

            var isArg1ConstantExpression = isArg1MemberExpression
                ? (arg1 as MemberExpression).Expression is ConstantExpression : false;
            var isArg2ConstantExpression = isArg2MemberExpression
                ? (arg2 as MemberExpression).Expression is ConstantExpression : false;

            var isAr2ConstantStringConstant = false;

            if (isArg2ConstantExpression)
            {
                var cn = (ConstantNode)GetParser(arg2).Parse(queryMapping);
                isAr2ConstantStringConstant = cn.ParameterType == typeof(string);
            }

            if (isArg1ConstantExpression && isArg2MemberExpression)
            {
                return ParseContainsInCollection(queryMapping);
            }
            else if (isArg1MemberExpression && isAr2ConstantStringConstant)
            {
                return ParseStringContains(queryMapping);
            }

            throw new NotSupportedException($"ContainsOrNullExpressionParser: {expression} not supported");
        }

        private Node ParseStringContains(IQueryMapping queryMapping)
        {
            var a1 = GetParser(expression.Arguments[0]);
            var a2 = GetParser(expression.Arguments[1]);

            var memberAccessResult = (MemberAccessNode)a1.Parse(queryMapping);
            var valueSetResult = (ConstantNode)a2.Parse(queryMapping);

            memberAccessResult.Formatter = (s) => "\"%\" + " + s + " + \"%\"";
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

        private Node ParseContainsInCollection(IQueryMapping queryMapping)
        {
            var valuesSet = GetParser(expression.Arguments[0]);
            var memberAccess = GetParser(expression.Arguments[1]);

            var valueSetResult = (ConstantNode)valuesSet.Parse(queryMapping);
            var memberAccessResult = (MemberAccessNode)memberAccess.Parse(queryMapping);

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
