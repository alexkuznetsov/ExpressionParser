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
    }


}
