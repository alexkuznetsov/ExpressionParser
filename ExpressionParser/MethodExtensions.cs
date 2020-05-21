using System;
using System.Collections.Generic;

namespace ExpressionParser
{
    public static class MethodExtensions
    {
#pragma warning disable IDE0060 // Remove unused parameter

        public static bool LikeOrNull(this string _, string ___) => throw new InvalidOperationException("");

        public static bool EqualsOrNull<T>(this T node, object value) => throw new InvalidOperationException("");

        public static bool ContainsOrNull<TSource>(this IEnumerable<TSource> source, TSource value) => throw new InvalidOperationException("");
        public static bool ContainsOrNull(this string source, string value) => throw new InvalidOperationException("");

#pragma warning restore IDE0060 // Remove unused parameter
    }
}
