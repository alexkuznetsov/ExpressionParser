using System;
using System.Collections;
using ExpressionParser.AST.Enum;

namespace ExpressionParser.AST
{
    public class EmptyNode : Node
    {
        public override void Visit(NodeExpression finalExpression, IQueryMapping mapping)
        {
            throw new NotImplementedException();
        }
    }

    public class BinaryNode : Node
    {
        public BinaryNode(Operation operation)
        {
            Operation = operation;
        }

        public Node LeftNode { get; set; }

        public Node RightNode { get; set; }

        public Operation Operation { get; }

        [Obsolete("Выделить визиторы")]
        public override void Visit(NodeExpression finalExpression, IQueryMapping mapping)
        {
            if (LeftNode is BinaryNode)
            {
                finalExpression.Append('(');
                LeftNode.Visit(finalExpression, mapping);
                finalExpression.Append(' ');
                finalExpression.Append(Operation.AsString());
                finalExpression.Append(' ');
                RightNode.Visit(finalExpression, mapping);
                finalExpression.Append(')');
            }
            else if (LeftNode is MemberAccessNode me && RightNode is ConstantNode ce)
            {
                finalExpression.Append('(');

                var memberName = me.Parent != null ? $"{me.Parent.MemberName}.{me.MemberName}" : me.MemberName;

                if (me.MemberName.StartsWith('@'))
                {
                    var parameterName = (me.Parent != null ? $"{me.Parent.MemberName}{me.MemberName}" : me.MemberName)
                        .Replace(".", "")
                        .Replace("@", "");

                    var type = ce.ParameterType;

                    if (type != typeof(string) && (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type)))
                    {
                        parameterName += "Collection";
                    }
                    finalExpression.Append('@');
                    finalExpression.Append(parameterName);
                }
                else
                {
                    if (!mapping.Mappings.TryGetValue(memberName, out var identifier))
                    {
                        identifier = memberName;
                    }

                    finalExpression.Append(identifier);
                }

                if (ce.Value is null && !ce.ForceParameter)
                {
                    finalExpression.Append(' ');
                    finalExpression.Append(Operation == Operation.Equal ? "IS NULL" : "IS NOT NULL");
                }
                else
                {
                    finalExpression.Append(' ');
                    finalExpression.Append(Operation.AsString());
                    finalExpression.Append(' ');

                    var parameterName = memberName.Replace(".", "");
                    var type = ce.ParameterType;

                    if (type != typeof(string) && (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type)))
                    {
                        parameterName += "Collection";
                    }

                    if (me.Formatter != null)
                    {
                        finalExpression.Append(me.Formatter($"@{parameterName}"));
                    }
                    else
                    {
                        finalExpression.Append($"@{parameterName}");
                    }
                    finalExpression.Parameters.Add(new NodeParameter(parameterName, ce.Value));
                }
                finalExpression.Append(')');
            }
            else if (LeftNode is MethodCallNode methodCallNode && RightNode is ConstantNode methodCallArg)
            {
                if (!mapping.Mappings.TryGetValue(methodCallNode.MemberName, out var identifier))
                {
                    identifier = methodCallNode.MemberName;
                }

                finalExpression.Append('(');
                finalExpression.Append(identifier);

                finalExpression.Append(' ');
                finalExpression.Append(Operation.AsString());
                finalExpression.Append(' ');

                var parameterName = methodCallNode.MemberName.Replace(".", "");
                var type = methodCallArg.ParameterType;


                if (type != typeof(string) && (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type)))
                {
                    parameterName += "Collection";
                }

                finalExpression.Append(methodCallNode.Formatter($"@{parameterName}"));

                finalExpression.Parameters.Add(new NodeParameter(parameterName, methodCallArg.Value));
                finalExpression.Append(')');
            }
            else if (LeftNode is ConstantNode c1 && RightNode is ConstantNode c2)
            {
                finalExpression.Append('(');
                finalExpression.Append(c1.Value);
                finalExpression.Append(' ');
                finalExpression.Append(Operation.AsString());
                finalExpression.Append(' ');
                finalExpression.Append(c2.Value);
                finalExpression.Append(')');
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }


}
