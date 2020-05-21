﻿using System.Linq.Expressions;

using ExpressionParser.AST;

namespace ExpressionParser.Linq
{
    internal class LambdaExpressionParser : Parser
    {
        private readonly LambdaExpression node;

        public LambdaExpressionParser(LambdaExpression node) => this.node = node;

        public override Node Parse(IQueryMapping queryMapping) => GetParser(node.Body).Parse(queryMapping);
    }
}
