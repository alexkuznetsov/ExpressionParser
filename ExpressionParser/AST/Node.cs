namespace ExpressionParser.AST
{
    public abstract class Node
    {
        public abstract void Visit(NodeExpression finalExpression, IQueryMapping mapping);
    }


}
