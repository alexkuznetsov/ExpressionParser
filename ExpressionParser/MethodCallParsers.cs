using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using ExpressionParser.Linq;

namespace ExpressionParser
{
    internal static class MethodCallParsers
    {
        class Test
        {
            public Test(Func<MethodCallExpression, bool> canAccept, Func<MethodCallExpression, Parser> builder)
            {
                CanAccept = canAccept;
                Builder = builder;
            }

            public Func<MethodCallExpression, bool> CanAccept { get; }
            public Func<MethodCallExpression, Parser> Builder { get; }
        }

        private static readonly List<Test> mappingList = new List<Test>();

        private static IReadOnlyCollection<Test> Mapping => mappingList;

        private static readonly object mappingLock = new object();

        public static void AddMapping(Func<MethodCallExpression, bool> canAcceptTest, Func<MethodCallExpression, Parser> factory)
        {
            lock (mappingLock)
            {
                mappingList.Add(new Test(canAcceptTest, factory));
            }
        }

        static MethodCallParsers()
        {
            AddMapping((e) => e.Arguments.Count == 1,
                       (e) => new DefaultMethodCallExpressionParser(e));

            AddMapping((e) => e.Arguments.Count == 2 && e.Method.Name == "Contains",
                       (e) => new ContainsInCollectionExpressionParser(e));

            AddMapping((e) => e.Arguments.Count == 2 && e.Method.Name == "ContainsOrNull",
                       (e) => new ContainsOrNullExpressionParser(e));

            AddMapping((e) => e.Arguments.Count == 2 && e.Method.Name == "LikeOrNull",
                       (e) => new LikeOrNullExpressionParser(e));

            AddMapping((e) => e.Arguments.Count == 2 && e.Method.Name == "EqualsOrNull",
                       (e) => new EqualsOrNullExpressionParser(e));
        }

        public static Parser DetectWhoCanAccept(MethodCallExpression expression)
        {
            var parserTest = Mapping.FirstOrDefault(x => x.CanAccept(expression));

            if (parserTest != null)
            {
                return parserTest.Builder(expression);
            }

            throw new NotSupportedException($"Method {expression.Method} supported only one argument, constant");
        }
    }
}
