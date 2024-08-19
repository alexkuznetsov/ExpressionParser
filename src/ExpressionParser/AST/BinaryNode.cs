namespace ExpressionParser.AST;
/// <summary>
/// AST tree node for binary node, e.g. a+b
/// </summary>

public class BinaryNode : Node
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="operation"></param>
    public BinaryNode(Operation operation)
    {
        Operation = operation;
    }

    /// <summary>
    /// Left side
    /// </summary>
    public Node LeftNode { get; set; }

    /// <summary>
    /// Right side
    /// </summary>
    public Node RightNode { get; set; }

    /// <summary>
    /// operation
    /// </summary>
    public Operation Operation { get; }
}
