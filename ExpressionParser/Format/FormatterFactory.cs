using System;

using ExpressionParser.AST;

namespace ExpressionParser.Format
{
    internal class FormatterFactory
    {
        public Func<Node, bool> CanAccept { get; set; }
        public Func<Node, SqlFormatter> Builder { get; set; }
    }
}
