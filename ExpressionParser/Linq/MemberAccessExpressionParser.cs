using System;
using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq
{
    internal class MemberAccessExpressionParser : Parser
    {
        private readonly MemberExpression expression;

        public MemberAccessExpressionParser(MemberExpression expression)
        {
            this.expression = expression;
        }

        //private Expression NodeInBottom()
        //{
        //    var e = expression.Expression;
        //    while(e.NodeType != ExpressionType.Parameter || e!=null)
        //    {
        //        e = E
        //    }

        //    return e;
        //}

        public override Node Parse(IQueryMapping queryMapping)
        {
            var memberName = expression.GetPropertyName('.');
            //if (expression.Expression.NodeType == ExpressionType.MemberAccess)
            //{
            //    var subNode = (MemberAccessNode)GetParser(expression.Expression).Parse(queryMapping);
            //    memberName = subNode.MemberName + memberName;
            //}

            if (expression.Expression is ConstantExpression)
            {
                if (expression.Expression.NodeType == ExpressionType.Constant)
                {
                    var objectMember = Expression.Convert(expression, typeof(object));
                    var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                    var getter = getterLambda.Compile();
                    var targetValue = getter();

                    return new ConstantNode(expression.Type, targetValue);
                }
            }

            if (!queryMapping.Mappings.TryGetValue(memberName, out var replacement))
                replacement = memberName;

            return new MemberAccessNode(expression.Type, replacement, memberName);
        }

        
    }
}
