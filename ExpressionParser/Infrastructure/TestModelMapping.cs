
using Domain;

using ExpressionParser.Parser;

namespace Infrastructure
{
    public class TestModelMapping : QueryMapping<TestModel>
    {
        public TestModelMapping() : base("m")
        {
            AutoMapAllProperties();
        }
    }
}
