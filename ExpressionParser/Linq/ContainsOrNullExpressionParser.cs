using System;
using System.Collections;
using System.Linq;
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

        public override Node Parse()
        {
            var arg1 = GetParser(expression.Arguments[0]).Parse();
            var arg2 = GetParser(expression.Arguments[1]).Parse();

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
              MemberAccessNode memberNode
            , ConstantNode valNode)
        {
            //var a1 = GetParser(expression.Arguments[0]);
            //var a2 = GetParser(expression.Arguments[1]);

            //var memberAccessResult = (MemberAccessNode)a1.Parse(queryMapping);
            //var valueSetResult = (ConstantNode)a2.Parse(queryMapping);

            memberNode.Formatter = (s) => "'%' + " + s + " + '%'";
            valNode.ForceParameter = true;

            return new BinaryNode(Operation.OrElse) /* property like @val or @val is null */
            {
                LeftNode = new BinaryNode(Operation.Like)
                {
                    LeftNode = memberNode,
                    RightNode = valNode
                },
                RightNode = new BinaryNode(Operation.Equal)
                {
                    LeftNode = new MemberAccessNode(valNode.ParameterType, $"@{memberNode.MemberName}", memberNode.Parent),
                    RightNode = new ConstantNode(valNode.ParameterType, null)
                }
            };
        }

        private Node ParseContainsInCollection( 
              ConstantNode valNode
            , MemberAccessNode memberNode)
        {
            //var valuesSet = GetParser(expression.Arguments[0]);
            //var memberAccess = GetParser(expression.Arguments[1]);

            //var valueSetResult = (ConstantNode)valuesSet.Parse(queryMapping);
            //var memberAccessResult = (MemberAccessNode)memberAccess.Parse(queryMapping);

            if ((valNode.Value == null)
                ||
                (valNode.Value is IEnumerable e && (e.Cast<object>().Any() == false)))
            {
                return new BinaryNode(Operation.Equal)
                {
                    LeftNode = new ConstantNode(valNode.ParameterType, 1),
                    RightNode = new ConstantNode(valNode.ParameterType, 1)
                };
            }
            
            return new BinaryNode(Operation.In)
            {
                LeftNode = memberNode,
                RightNode = valNode
            };
        }
    }
}
