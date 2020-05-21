
using Domain;

using ExpressionParser;

namespace Infrastructure
{
    public class TestModelMapping : QueryMapping<TestModel>
    {
        public TestModelMapping() : base("m")
        {
            AutoMapAllProperties();
            Map(x => x.SubModel.Name, "name", "s");
        }
    }
}
