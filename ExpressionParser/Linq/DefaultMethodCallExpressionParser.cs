using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using ExpressionParser.AST;
using ExpressionParser.AST.Enum;

namespace ExpressionParser.Linq
{
    internal class DefaultMethodCallExpressionParser : Parser
    {
        private readonly Dictionary<string, SqlOperationFormatter> sqlOperationMap = new Dictionary<string, SqlOperationFormatter>
        {
            {"StartsWith", new SqlOperationFormatter(Operation.Like, (s) =>s + " + '%'")},
            {"Contains",    new SqlOperationFormatter(Operation.Like, (s) =>"'%' + " + s + " + '%'")},
            {"EndsWith",    new SqlOperationFormatter(Operation.Like, (s) =>"'%' + " + s)},
        };

        private class SqlOperationFormatter
        {
            public SqlOperationFormatter(Operation operation, Func<string, string> formatter)
            {
                Operation = operation;
                Formatter = formatter;
            }

            public Operation Operation { get; }
            public Func<string, string> Formatter { get; }
        }

        private readonly MethodCallExpression expression;

        public DefaultMethodCallExpressionParser(MethodCallExpression expression)
        {
            this.expression = expression;
        }

        public override Node Parse(IQueryMapping queryMapping)
        {
            var testObject = expression.Object as MemberExpression;

            if (!sqlOperationMap.TryGetValue(expression.Method.Name, out var formatter))
            {
                throw new NotSupportedException($"Method {expression.Method} not supported");
            }

            if (!queryMapping.Mappings.TryGetValue(testObject.Member.Name, out var leftNodeIdentifier))
                leftNodeIdentifier = testObject.Member.Name;


            return new BinaryNode(formatter.Operation)
            {
                LeftNode = new MethodCallNode
                {
                    Identifier = leftNodeIdentifier,
                    MemberName = testObject.Member.Name,
                    Formatter = formatter.Formatter
                },

                RightNode = GetParser(expression.Arguments[0]).Parse(queryMapping)
            };
        }
    }
}
