using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ExpressionParser
{
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
}
