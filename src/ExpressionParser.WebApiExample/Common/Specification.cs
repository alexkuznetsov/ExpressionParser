using System.Linq.Expressions;

namespace ExpressionParser.WebApiExample.Common;

internal class Specification<T> : ISpecification<T>
{
    public static readonly Specification<T> Empty = new Specification<T>(null!);

    public Specification(Expression<Func<T, bool>> expreression)
    {
        Criteria = expreression;
    }
    public Expression<Func<T, bool>> Criteria { get; }

    public int? PageSize { get; set; }
    public int? Start { get; set; }
}


internal interface ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; }

    public int? PageSize { get; }

    public int? Start { get; }
}
