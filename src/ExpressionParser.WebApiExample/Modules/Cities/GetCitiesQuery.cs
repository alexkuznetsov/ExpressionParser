using ExpressionParser.WebApiExample.Common;

namespace ExpressionParser.WebApiExample.Modules.Cities;

internal class GetCitiesQuery(GetCitiesQuery.FilterData filter) : IMessage<GetCitiesQuery.Result>
{
    public FilterData Filter { get; } = filter;

    public record FilterData(string? Name, string? Provice, int? Limit, int? Offset);
    public class FilterCriteria(FilterData filterData)
        : Specification<City>(x => x.Name.LikeOrNull(filterData.Name) &&
                        x.Province.LikeOrNull(filterData.Provice))
    {

    }

    public record Result(CityResponse[] Cities);
    public record CityResponse(int Id, string Name, string Province);

    public sealed class Handler(ICitiesRepository repository) : IMessageHandler<GetCitiesQuery, Result>
    {
        public async Task<Result> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
        {
            var filter = new FilterCriteria(request.Filter)
            {
                PageSize = request.Filter.Limit,
                Start = request.Filter.Offset
            };

            var result = await repository.GetAsync(filter, cancellationToken);

            return new Result(result.Select(c => new CityResponse(c.Id, c.Name, c.Province)).ToArray());
        }
    }
}
