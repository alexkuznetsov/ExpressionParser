using System.Diagnostics.CodeAnalysis;

namespace ExpressionParser.WebApiExample.Common;

internal interface IRepository<TModel>
{
    Task<TModel?> FindOneAsync(object id, CancellationToken cancellationToken = default);

    Task<ICollection<TModel>> GetAsync([NotNull] ISpecification<TModel> specification, CancellationToken cancellationToken = default);

    Task<int> CountAsync([NotNull] ISpecification<TModel> specification, CancellationToken cancellationToken = default);

    Task<TModel> CreateAsync([NotNull] TModel model, CancellationToken cancellationToken = default);

    Task<TModel> UpdateAsync([NotNull] TModel model, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync([NotNull] TModel model, CancellationToken cancellationToken = default);
}
