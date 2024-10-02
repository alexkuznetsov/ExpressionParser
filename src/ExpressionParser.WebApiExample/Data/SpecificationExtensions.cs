using ExpressionParser.WebApiExample.Common;
using System.Text;

namespace ExpressionParser.WebApiExample.Data;

static class SpecificationExtensions
{
    /// <summary>
    /// Parse specificaiton as where sql and return parameters dictionary
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specification"></param>
    /// <param name="sql"></param>
    /// <param name="mapping"></param>
    /// <returns></returns>
    public static Dictionary<string, object?> AsWhereSql<T>(this ISpecification<T> specification, 
        StringBuilder sql, QueryMapping<T> mapping)
    {
        var whereParams = new Dictionary<string, object?>();

        if (specification.Criteria != null)
        {
            var parser = Parser.GetParser(specification.Criteria);
            var node = parser.Parse();
            var result = Parser.CreateResult(node, mapping);

            sql.AppendLine("and");
            sql.AppendLine(result.ResultExpression);

            foreach (var i in result.Parameters)
            {
                whereParams.Add(i.Name, i.Value);
            }
        }

        return whereParams;
    }
}
