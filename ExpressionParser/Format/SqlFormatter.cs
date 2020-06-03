using System;
using System.Collections.Generic;
using System.Linq;

using ExpressionParser.AST;

namespace ExpressionParser.Format
{
    internal abstract class SqlFormatter
    {
        private static readonly List<FormatterFactory>
            formatterFactories = new List<FormatterFactory>();

        static SqlFormatter()
        {
            formatterFactories.Add(new FormatterFactory
            {
                CanAccept = new Func<Node, bool>((node) => node is BinaryNode binary && binary.LeftNode is BinaryNode && binary.RightNode is BinaryNode),
                Builder = new Func<Node, SqlFormatter>((node) => new BinaryNodeFormater((BinaryNode)node))
            });

            formatterFactories.Add(new FormatterFactory
            {
                CanAccept = new Func<Node, bool>((node) => node is BinaryNode binary && binary.LeftNode is MemberAccessNode && binary.RightNode is ConstantNode),
                Builder = new Func<Node, SqlFormatter>((node) => new MemberAccessFormatter((BinaryNode)node))
            });

            formatterFactories.Add(new FormatterFactory
            {
                CanAccept = new Func<Node, bool>((node) => node is BinaryNode binary && binary.LeftNode is MethodCallNode && binary.RightNode is ConstantNode),
                Builder = new Func<Node, SqlFormatter>((node) => new MethodCallFormatter((BinaryNode)node))
            });

            formatterFactories.Add(new FormatterFactory
            {
                CanAccept = new Func<Node, bool>((node) => node is BinaryNode binary && binary.LeftNode is ConstantNode && binary.RightNode is ConstantNode),
                Builder = new Func<Node, SqlFormatter>((node) => new ConstantsNodesFormatter((BinaryNode)node))
            });
        }

        public static SqlFormatter GetForNode(Node node)
        {
            var builder = formatterFactories.Where(x => x.CanAccept(node))
                .Select(x => x.Builder)
                .SingleOrDefault();

            if (builder != null)
            {
                return builder.Invoke(node);
            }

            throw new InvalidOperationException();
        }

        public abstract void Format(NodeExpression finalExpression, IQueryMapping mapping);

        internal static string OperationAsString(Operation operation)
        {
            return operation switch
            {
                Operation.Equal => "=",
                Operation.AndAlso => "AND",
                Operation.OrElse => "OR",
                Operation.NotEqual => "!=",
                Operation.GreaterThan => ">",
                Operation.GreaterThanOrEqual => ">=",
                Operation.LessThan => "<",
                Operation.LessThanOrEqual => "<=",
                Operation.Negate => "-",
                Operation.NegateChecked => "-",
                Operation.Not => "!",
                Operation.Like => "LIKE",
                Operation.In => "IN",
                _ => throw new NotSupportedException($"Operation {operation} not supported"),
            };
        }
    }
}