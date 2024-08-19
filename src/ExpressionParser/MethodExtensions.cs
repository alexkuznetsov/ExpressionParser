using System;
using System.Collections.Generic;

namespace ExpressionParser;

/// <summary>
/// Extensions
/// </summary>
public static class MethodExtensions
{
    /// <summary>
    /// It must be similar like the string or string must be null
    /// </summary>
    /// <param name="_"></param>
    /// <param name="___"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static bool LikeOrNull(this string _, string ___) => throw new InvalidOperationException("");

    /// <summary>
    ///  It must be equals for the string or string must be null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static bool EqualsOrNull<T>(this T node, object value) => throw new InvalidOperationException("");

    /// <summary>
    /// It must contains the string or string must be null
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static bool ContainsOrNull<TSource>(this IEnumerable<TSource> source, TSource value) => throw new InvalidOperationException("");

    /// <summary>
    /// It must contains the string or string must be null
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static bool ContainsOrNull(this string source, string value) => throw new InvalidOperationException("");
}
