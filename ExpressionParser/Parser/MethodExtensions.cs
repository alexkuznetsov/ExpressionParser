using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Parser
{
    public static class MethodExtensions
    {
        public static bool LikeOrNull(this string _, string ___) => throw new InvalidOperationException("");

        public static bool EqualsOrNull<T>(this T node, object value) => throw new InvalidOperationException("");

        public static bool ContainsOrNull<TSource>(this IEnumerable<TSource> source, TSource value) => throw new InvalidOperationException("");
    }
}
