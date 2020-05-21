namespace ExpressionParser
{
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


}
