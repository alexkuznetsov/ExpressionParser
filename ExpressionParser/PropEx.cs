using System;
using System.Linq.Expressions;

namespace ExpressionParser
{
    internal static class PropEx
    {
        public static string GetPropertyName/*<TModel, TValue>*/(this Expression/*<Func<TModel, TValue>>*/ propertySelector, char delimiter = '.', char endTrim = ')')
        {
            while (propertySelector.NodeType != ExpressionType.MemberAccess)
            {
                propertySelector = PopMemberAccess(propertySelector);
            }
            var asString = propertySelector.ToString(); // gives you: "o => o.Whatever"
            var firstDelim = asString.IndexOf(delimiter); // make sure there is a beginning property indicator; the "." in "o.Whatever" -- this may not be necessary?

            return firstDelim < 0
                ? asString
                : asString.Substring(firstDelim + 1).TrimEnd(endTrim);
        }//--   fn  GetPropertyNameExtended

        private static Expression PopMemberAccess(Expression expression)
        {
            switch(expression.NodeType)
            {
                case ExpressionType.Lambda:
                    return ((LambdaExpression)expression).Body;
                case ExpressionType.Convert:
                    return ((UnaryExpression)expression).Operand;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
