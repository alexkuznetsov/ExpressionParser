using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionParser.Parser
{
    public abstract class ExpressionParser
    {
        public static ExpressionParser GetParserForExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    return new ConstantExpressionParser((ConstantExpression)expression);
                case ExpressionType.Lambda:
                    return new LambdaExpressionParser((LambdaExpression)expression);
                case ExpressionType.Add:
                case ExpressionType.OrElse:
                case ExpressionType.AndAlso:
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                    return new BinaryExpressionParser((BinaryExpression)expression);
                case ExpressionType.MemberAccess:
                    return new MemberAccessExpressionParser((MemberExpression)expression);
                case ExpressionType.Convert:
                    return new UnaryExpressionParser((UnaryExpression)expression);
                case ExpressionType.Call:
                    return new MethodCallExpressionParser((MethodCallExpression)expression);
                default:
                    throw new NotImplementedException($"Parser for {expression.NodeType} not implemented");
            }
        }

        public static NodeExpression CreateResult(ExpressionNode node)
        {
            var finalExpression = new NodeExpression();
            node.Visit(finalExpression);

            return finalExpression;
        }

        public abstract ExpressionNode Parse(IQueryMapping queryMapping);

        protected NodeOperation ParseOperation(ExpressionType expressionType)
        {
            return expressionType switch
            {
                ExpressionType.Equal => NodeOperation.Equal /*"="*/,
                ExpressionType.AndAlso => NodeOperation.AndAlso/*"and"*/,
                ExpressionType.OrElse => NodeOperation.OrElse/*"or"*/,
                ExpressionType.NotEqual => NodeOperation.NotEqual/*"!="*/,
                ExpressionType.GreaterThan => NodeOperation.GreaterThan/*">"*/,
                ExpressionType.GreaterThanOrEqual => NodeOperation.GreaterThanOrEqual/* ">="*/,
                ExpressionType.LessThan => NodeOperation.LessThan /*"<"*/,
                ExpressionType.LessThanOrEqual => NodeOperation.LessThanOrEqual/*"<="*/,
                ExpressionType.Negate => NodeOperation.Negate/*"-"*/,
                ExpressionType.NegateChecked => NodeOperation.NegateChecked/*"-"*/,
                ExpressionType.Not => NodeOperation.Not/*"!"*/,

                _ => throw new NotImplementedException($"Operation {expressionType} not supported"),
            };
        }
    }

    public class NodeExpression
    {
        private readonly StringBuilder buffer = new StringBuilder();

        public List<NodeParameter> Parameters { get; } = new List<NodeParameter>();

        public string ResultExpression
        {
            get
            {
                return buffer.ToString();
            }
        }

        [DebuggerStepThrough]
        public void Append(char data) => buffer.Append(data);
        [DebuggerStepThrough]
        public void Append(string data) => buffer.Append(data);
    }

    #region Parsers

    public class BinaryExpressionParser : ExpressionParser
    {
        private readonly BinaryExpression node;

        public BinaryExpressionParser(BinaryExpression node)
        {
            this.node = node;
        }

        public override ExpressionNode Parse(IQueryMapping queryMapping)
        {
            var leftNode = GetParserForExpression(node.Left).Parse(queryMapping);
            var rightNode = GetParserForExpression(node.Right).Parse(queryMapping);

            return new BinaryNode(ParseOperation(node.NodeType))
            {
                LeftNode = leftNode,
                RightNode = rightNode
            };
        }
    }

    public class ConstantExpressionParser : ExpressionParser
    {
        private readonly ConstantExpression expression;

        public ConstantExpressionParser(ConstantExpression expression)
        {
            this.expression = expression;
        }

        public override ExpressionNode Parse(IQueryMapping queryMapping)
        {
            var type = expression.Type;
            return new ConstantNode(type, expression.Value);
        }
    }

    public class LambdaExpressionParser : ExpressionParser
    {
        private readonly LambdaExpression node;

        public LambdaExpressionParser(LambdaExpression node)
        {
            this.node = node;
        }

        public override ExpressionNode Parse(IQueryMapping queryMapping)
        {
            // Visit the body:
            var bodyParser = ExpressionParser.GetParserForExpression(node.Body);

            return bodyParser.Parse(queryMapping);
        }
    }

    public class MemberAccessExpressionParser : ExpressionParser
    {
        private readonly MemberExpression expression;

        public MemberAccessExpressionParser(MemberExpression expression)
        {
            this.expression = expression;
        }

        public override ExpressionNode Parse(IQueryMapping queryMapping)
        {
            if (expression.Expression != null && expression.Expression.NodeType != ExpressionType.Parameter)
            {
                if (expression.Expression.NodeType == ExpressionType.Constant)
                {
                    var objectMember = Expression.Convert(expression, typeof(object));
                    var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                    var getter = getterLambda.Compile();
                    var targetValue = getter();

                    return new ConstantNode(expression.Type, targetValue);
                }
            }

            if (!queryMapping.Mappings.TryGetValue(expression.Member.Name, out var replacement))
                replacement = expression.Member.Name;

            return new MemberAccessNode(expression.Type, replacement, expression.Member.Name);
        }
    }

    public class MethodCallExpressionParser : ExpressionParser
    {
        private readonly MethodCallExpression expression;

        public MethodCallExpressionParser(MethodCallExpression expression)
        {
            this.expression = expression;
        }

        readonly Dictionary<string, SqlOperationFormatter> sqlOperationMap = new Dictionary<string, SqlOperationFormatter>
        {
            {"StartsWidth", new SqlOperationFormatter(NodeOperation.Like, (s) =>s + " + \"%\"")},
            {"Contains",    new SqlOperationFormatter(NodeOperation.Like, (s) =>"\"%\" + " + s + " + \"%\'\"")},
            {"EndsWith",    new SqlOperationFormatter(NodeOperation.Like, (s) =>"\"%\" + " + s)},
        };

        class SqlOperationFormatter
        {
            public SqlOperationFormatter(NodeOperation operation, Func<string, string> formatter)
            {
                Operation = operation;
                Formatter = formatter;
            }

            public NodeOperation Operation { get; }
            public Func<string, string> Formatter { get; }
        }

        public override ExpressionNode Parse(IQueryMapping queryMapping)
        {
            var method = expression.Method;
            var testObject = expression.Object as MemberExpression;

            if (expression.Arguments.Count == 1)
            {
                if (!sqlOperationMap.TryGetValue(method.Name, out var formatter))
                {
                    throw new NotSupportedException($"Method {method} not supported");
                }

                if (!queryMapping.Mappings.TryGetValue(testObject.Member.Name, out var leftNodeIdentifier))
                    leftNodeIdentifier = testObject.Member.Name;

                var result = new BinaryNode(formatter.Operation)
                {
                    LeftNode = new MethodCallNode
                    {
                        Identifier = leftNodeIdentifier,
                        MemberName = testObject.Member.Name,
                        Formatter = formatter.Formatter
                    }
                };

                result.RightNode = GetParserForExpression(expression.Arguments[0]).Parse(queryMapping);

                return result;
            }
            else if (expression.Arguments.Count == 2 && method.Name == "Contains")
            {
                return new ContainsInCollectionExpressionParser(expression).Parse(queryMapping);
            }
            else if (expression.Arguments.Count == 2 && method.Name == "ContainsOrNull")
            {
                return new ContainsOrNullExpressionParser(expression).Parse(queryMapping);
            }
            else if (expression.Arguments.Count == 2 && method.Name == "LikeOrNull")
            {
                return new LikeOrNullExpressionParser(expression).Parse(queryMapping);
            }
            else if (expression.Arguments.Count == 2 && method.Name == "EqualsOrNull")
            {
                return new EqualsOrNullExpressionParser(expression).Parse(queryMapping);
            }

            throw new NotSupportedException($"Method {method} supported only one argument, constant");
        }
    }

    public class ContainsOrNullExpressionParser : ExpressionParser
    {
        private readonly MethodCallExpression expression;

        public ContainsOrNullExpressionParser(MethodCallExpression expression)
        {
            this.expression = expression;
        }

        public override ExpressionNode Parse(IQueryMapping queryMapping)
        {
            var valuesSet = GetParserForExpression(expression.Arguments[0]);
            var memberAccess = GetParserForExpression(expression.Arguments[1]);

            var valueSetResult = (ConstantNode)valuesSet.Parse(queryMapping);
            var memberAccessResult = (MemberAccessNode)memberAccess.Parse(queryMapping);

            return new BinaryNode(NodeOperation.OrElse) /* property in @collection or @collection is null */
            {
                LeftNode = new BinaryNode(NodeOperation.In)
                {
                    LeftNode = memberAccessResult,
                    RightNode = valueSetResult
                },
                RightNode = new BinaryNode(NodeOperation.Equal)
                {
                    LeftNode = new MemberAccessNode(valueSetResult.ParameterType, $"@{memberAccessResult.MemberName}", memberAccessResult.MemberName),
                    RightNode = new ConstantNode(valueSetResult.ParameterType, null)
                }
            };
        }
    }

    public class EqualsOrNullExpressionParser : ExpressionParser
    {
        private readonly MethodCallExpression expression;

        public EqualsOrNullExpressionParser(MethodCallExpression expression)
        {
            this.expression = expression;
        }

        public override ExpressionNode Parse(IQueryMapping queryMapping)
        {
            var propertyAccess = (MemberAccessNode)GetParserForExpression(expression.Arguments[0]).Parse(queryMapping);
            var values = (ConstantNode)GetParserForExpression(expression.Arguments[1]).Parse(queryMapping);
            values.ForceParameter = true;

            return new BinaryNode(NodeOperation.OrElse)
            {
                LeftNode = new BinaryNode(NodeOperation.Equal)
                {
                    LeftNode = propertyAccess,
                    RightNode = values
                },
                RightNode = new BinaryNode(NodeOperation.Equal)
                {
                    LeftNode = new MemberAccessNode(propertyAccess.MemberType, $"@{propertyAccess.MemberName}", propertyAccess.MemberName),
                    RightNode = new ConstantNode(propertyAccess.MemberType, null)
                }
            };
        }
    }

    public class LikeOrNullExpressionParser : ExpressionParser
    {
        private readonly MethodCallExpression expression;

        public LikeOrNullExpressionParser(MethodCallExpression expression)
        {
            this.expression = expression;
        }

        public override ExpressionNode Parse(IQueryMapping queryMapping)
        {
            var propertyAccess = (MemberAccessNode)GetParserForExpression(expression.Arguments[0]).Parse(queryMapping);

            propertyAccess.Formatter = (s) => @$"""%"" + {s} + ""%""";

            var values = (ConstantNode)GetParserForExpression(expression.Arguments[1]).Parse(queryMapping);
            values.ForceParameter = true;

            return new BinaryNode(NodeOperation.OrElse)
            {
                LeftNode = new BinaryNode(NodeOperation.Like)
                {
                    LeftNode = propertyAccess,
                    RightNode = values
                },
                RightNode = new BinaryNode(NodeOperation.Equal)
                {
                    LeftNode = new MemberAccessNode(propertyAccess.MemberType, $"@{propertyAccess.MemberName}", propertyAccess.MemberName),
                    RightNode = new ConstantNode(propertyAccess.MemberType, null)
                }
            };
        }
    }

    public class ContainsInCollectionExpressionParser : ExpressionParser
    {
        private readonly MethodCallExpression expression;

        public ContainsInCollectionExpressionParser(MethodCallExpression expression)
        {
            this.expression = expression;
        }

        public override ExpressionNode Parse(IQueryMapping queryMapping)
        {
            var valuesSet = GetParserForExpression(expression.Arguments[0]);
            var memberAccess = GetParserForExpression(expression.Arguments[1]);

            var valueSetResult = valuesSet.Parse(queryMapping);
            var memberAccessResult = memberAccess.Parse(queryMapping);

            return new BinaryNode(NodeOperation.In)
            {
                LeftNode = memberAccessResult,
                RightNode = valueSetResult
            };
        }
    }

    public class UnaryExpressionParser : ExpressionParser
    {
        private readonly UnaryExpression expression;

        public UnaryExpressionParser(UnaryExpression expression)
        {
            this.expression = expression;
        }

        public override ExpressionNode Parse(IQueryMapping queryMapping)
        {
            if (expression.NodeType == ExpressionType.Convert)
            {
                return GetParserForExpression(expression.Operand).Parse(queryMapping);
            }
            else
            {
                throw new InvalidOperationException("UnaryExpressionParser: operand can be processed, but logic not implemented: " + expression);
            }
        }
    }

    #endregion

    #region Nodes

    public enum NodeOperation
    {
        AndAlso = 3,
        Coalesce = 7,
        Equal = 13,
        GreaterThan = 15,
        GreaterThanOrEqual = 16,
        LessThan = 20,
        LessThanOrEqual = 21,
        Negate = 28,
        NegateChecked = 30,
        Not = 34,
        NotEqual = 35,
        OrElse = 37,


        /*Custom*/

        Like = 10037,
        In = 10038,
    }

    internal static class NodeOperationFormatter
    {
        internal static string AsString(this NodeOperation operation)
        {
            return operation switch
            {
                NodeOperation.Equal => "=",
                NodeOperation.AndAlso => "AND",
                NodeOperation.OrElse => "OR",
                NodeOperation.NotEqual => "!=",
                NodeOperation.GreaterThan => ">",
                NodeOperation.GreaterThanOrEqual => ">=",
                NodeOperation.LessThan => "<",
                NodeOperation.LessThanOrEqual => "<=",
                NodeOperation.Negate => "-",
                NodeOperation.NegateChecked => "-",
                NodeOperation.Not => "!",
                NodeOperation.Like => "LIKE",
                NodeOperation.In => "IN",
                _ => throw new NotImplementedException($"Operation {operation} not supported"),
            };
        }
    }


    public class BinaryNode : ExpressionNode
    {
        public BinaryNode(NodeOperation operation)
        {
            Operation = operation;
        }

        public ExpressionNode LeftNode { get; set; }

        public ExpressionNode RightNode { get; set; }

        public NodeOperation Operation { get; }

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
                    var parameterName = me.Identifier;
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
                    finalExpression.Append(Operation == NodeOperation.Equal ? "IS NULL" : "IS NOT NULL");
                }
                else
                {
                    finalExpression.Append(' ');
                    finalExpression.Append(Operation.AsString());
                    finalExpression.Append(' ');

                    var parameterName = me.MemberName;
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

                var parameterName = methodCallNode.MemberName;
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

    public abstract class ExpressionNode
    {
        public abstract void Visit(NodeExpression finalExpression);
    }

    public class ConstantNode : ExpressionNode
    {
        public object Value { get; }

        public bool ForceParameter { get; set; }

        public Type ParameterType { get; }

        public ConstantNode(Type type, object value)
        {
            ParameterType = type;
            Value = value;
        }

        public override void Visit(NodeExpression finalExpression) => throw new InvalidOperationException();
    }

    public class MemberAccessNode : ExpressionNode
    {
        public string Identifier { get; }

        public string MemberName { get; }

        public Func<string, string> Formatter { get; set; }

        public Type MemberType { get; }

        public MemberAccessNode(Type memberType, string identifier, string memberName)
        {
            MemberType = memberType;
            Identifier = identifier;
            MemberName = memberName;
        }

        public override void Visit(NodeExpression finalExpression) => throw new InvalidOperationException();
    }

    public class MethodCallNode : ExpressionNode
    {
        public string Identifier { get; internal set; }
        public string MemberName { get; internal set; }
        public Func<string, string> Formatter { get; internal set; }

        public override void Visit(NodeExpression finalExpression) => throw new InvalidOperationException();
    }

    #endregion

    #region Filter implementation

    public class NodeParameter
    {
        public NodeParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public object Value { get; }
    }

    #endregion
}
