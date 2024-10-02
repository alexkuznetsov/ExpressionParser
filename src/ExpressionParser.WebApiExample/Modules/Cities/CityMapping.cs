namespace ExpressionParser.WebApiExample.Modules.Cities;

class CityMapping : QueryMapping<City>
{
    public CityMapping() : base("x")
    {
        AutoMapAllProperties();
    }
}
