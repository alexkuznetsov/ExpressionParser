namespace ExpressionParser.AST
{
    public enum Operation
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
}
