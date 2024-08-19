using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ExpressionParser;

public class NodeExpression
{
    private readonly StringBuilder _buffer = new();

    public List<NodeParameter> Parameters { get; } = [];

    public string ResultExpression
    {
        get
        {
            return _buffer.ToString();
        }
    }

    public string ResultExpressionWithoutNamedParameters
    {
        get
        {
            var buff = new StringBuilder();
            buff.Append(_buffer);

            foreach (var o in Parameters)
            {
                buff.Replace($"@{o.Name}", "?");
            }

            return buff.ToString();
        }
    }

    [DebuggerStepThrough]
    public void Append(char data) => _buffer.Append(data);
    [DebuggerStepThrough]
    public void Append(string data) => _buffer.Append(data);
    [DebuggerStepThrough]
    public void Append(object data) => _buffer.Append(data);
}
