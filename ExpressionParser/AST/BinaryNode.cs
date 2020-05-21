using System;
using System.Collections;
using ExpressionParser.AST.Enum;

namespace ExpressionParser.AST
{
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
        public override void Visit(NodeExpression finalExpression)
        {
            if (LeftNode is BinaryNode)
            {
                finalExpression.Append('(');
                LeftNode.Visit(finalExpression);
                finalExpression.Append(' ');
                finalExpression.Append(Operation.AsString());
                finalExpression.Append(' ');
                RightNode.Visit(finalExpression);
                finalExpression.Append(')');
            }
            else if (LeftNode is MemberAccessNode me && RightNode is ConstantNode ce)
            {
                finalExpression.Append('(');
                if (me.Identifier.StartsWith('@'))
                {
                    var parameterName = me.Identifier.Replace(".", "");
                    var type = ce.ParameterType;

                    if (type != typeof(string) && (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type)))
                    {
                        parameterName += "Collection";
                    }
                    finalExpression.Append(parameterName);
                }
                else
                {
                    finalExpression.Append(me.Identifier);
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

                    var parameterName = me.MemberName.Replace(".", "");
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

                    //finalExpression.Append($"@{me.MemberName}");
                    //finalExpression.Parameters.Add(new Filter(me.MemberName, ce.Value));
                }
                finalExpression.Append(')');
            }
            else if (LeftNode is MethodCallNode methodCallNode && RightNode is ConstantNode methodCallArg)
            {
                finalExpression.Append('(');
                finalExpression.Append(methodCallNode.Identifier);

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
            else
            {
                throw new InvalidOperationException();
            }
        }
    }


}
