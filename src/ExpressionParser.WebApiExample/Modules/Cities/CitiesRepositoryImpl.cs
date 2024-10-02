using System.Diagnostics.CodeAnalysis;
using System.Text;

using ExpressionParser.WebApiExample.Common;
using ExpressionParser.WebApiExample.Data;

namespace ExpressionParser.WebApiExample.Modules.Cities;

internal sealed class CitiesRepositoryImpl(IDataSession dataSession, QueryMapping<City> mapping) : ICitiesRepository
{
    public Task<int> CountAsync([NotNull] ISpecification<City> specification, CancellationToken cancellationToken = default)
    {
        if (specification.PageSize > 0 && specification.Start >= 0)
        {
            var sql = new StringBuilder("""
                select count(id) from [cities] x where 
                x.is_deleted = 0
                """);

            var whereParams = specification.AsWhereSql(sql, mapping);

            return dataSession.ExecuteScalarAsync<int>(sql.ToString(), whereParams);
        }

        return Task.FromResult(-1);
    }

    public Task<City> CreateAsync([NotNull] City model, CancellationToken cancellationToken = default) 
        => throw new NotImplementedException();

    public Task<int> DeleteAsync([NotNull] City model, CancellationToken cancellationToken = default) 
        => throw new NotImplementedException();

    public Task<City?> FindOneAsync(object id, CancellationToken cancellationToken = default) 
        => throw new NotImplementedException();

    public Task<ICollection<City>> GetAsync([NotNull] ISpecification<City> specification, CancellationToken cancellationToken = default)
    {
        var sql = new StringBuilder("""
            select 
             x.rn
            ,x.id 
            ,x.name
            ,x.province
            
            from (select  ROW_NUMBER() over (partition by c.is_deleted order by name) as rn,  c.* FROM [cities] c) x 
            where x.is_deleted = 0
            """);

        var whereParams = specification.AsWhereSql(sql, mapping);

        sql.Append("""
            and ((x.rn between @pageBegin and @pageEnd) or (@pageBegin is null and @pageEnd is null))
            """);

        var withPaging = specification.Start >= 0 && specification.PageSize > 0;

        whereParams.Add("pageBegin", withPaging ? specification.Start : null);
        whereParams.Add("pageEnd", withPaging ? (specification.Start > 0 ? specification.Start - 1 : 0) + specification.PageSize : null);

        return dataSession.QueryAsync<City>(sql.ToString(), whereParams);
    }

    public Task<City> UpdateAsync([NotNull] City model, CancellationToken cancellationToken = default) 
        => throw new NotImplementedException();
}
