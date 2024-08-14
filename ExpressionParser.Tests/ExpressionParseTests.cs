using System;
using System.Linq;
using System.Linq.Expressions;

using Domain;

using Infrastructure;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace ExpressionParser.Tests;

public class ExpressionParseTests
{
    private IQueryMapping mapping;

    [SetUp]
    public void Setup()
    {
        mapping = new TestModelMapping();
    }

    [Test]
    public void GivenParentIdWithCodeWithName_ResultWithDefaultParentesis()
    {
        Expression<Func<TestModel, bool>> expression = s => s.ParentId == 1 && s.Code == "CODEXX1" || s.Name == "zzz";
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        Assert.That("(((m.parent_id = @ParentId) AND (m.code = @Code)) OR (m.name = @Name))", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(3));

        //Assert.That(parametes.ContainsKey("ParentId"));
        //Assert.That(parametes.ContainsKey("Code"));
        //Assert.That(parametes.ContainsKey("Name"));

        Assert.That(parametes, Contains.Key("ParentId"));
        Assert.That(parametes, Contains.Key("Code"));
        Assert.That(parametes, Contains.Key("Name"));

        Assert.That(parametes["ParentId"], Is.EqualTo(1));
        Assert.That(parametes["Code"], Is.EqualTo("CODEXX1"));
        Assert.That(parametes["Name"], Is.EqualTo("zzz"));
    }

    [Test]
    public void GivenParentIdWithCodeOrName_ResultWithExpectedParentesis()
    {
        Expression<Func<TestModel, bool>> expression = s => s.ParentId == 1 && (s.Code == "CODEXX1" || s.Name == "zzz");
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        Assert.That(@"((m.parent_id = @ParentId) AND ((m.code = @Code) OR (m.name = @Name)))", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(3));

        Assert.That(parametes, Contains.Key("ParentId"));
        Assert.That(parametes, Contains.Key("Code"));
        Assert.That(parametes, Contains.Key("Name"));

        Assert.That(parametes["ParentId"], Is.EqualTo(1));
        Assert.That(parametes["Code"], Is.EqualTo("CODEXX1"));
        Assert.That(parametes["Name"], Is.EqualTo("zzz"));
    }

    [Test]
    public void GivenCodeNotEquealsCODEXX1_ResultNotEqualsTranslatedSuccess()
    {
        Expression<Func<TestModel, bool>> expression = s => s.Code != "CODEXX1";
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        Assert.That(@"(m.code != @Code)", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(1));

        Assert.That(parametes, Contains.Key("Code"));

        Assert.That(parametes["Code"], Is.EqualTo("CODEXX1"));
    }

    [Test]
    public void GivenParentIdIsNotNull_ResultIsParentIdIsNotNull()
    {
        Expression<Func<TestModel, bool>> expression = s => s.ParentId != null;
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        Assert.That(@"(m.parent_id IS NOT NULL)", Is.EqualTo(result.ResultExpression));

        Assert.That(result.Parameters.Count, Is.EqualTo(0));
    }


    [Test]
    public void GivenParentIdIsNotNullAndIdGreater10_ResultIsParentIdIsNotNullAndIdGreaterIdParam()
    {
        Expression<Func<TestModel, bool>> expression = s => s.ParentId != null && s.Id > 10;
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        Assert.That(@"((m.parent_id IS NOT NULL) AND (m.id > @Id))", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(1));

        Assert.That(parametes, Contains.Key("Id"));

        Assert.That(parametes["Id"], Is.EqualTo(10));
    }

    [Test]
    public void GivenParentIdIsNotNullAndIdGreater10AndParentCodeStartsWith_ResultIsParentIdIsNotNullAndIdGreaterIdParamAndParentCodeLikeParam()
    {
        Expression<Func<TestModel, bool>> expression = s =>
            s.ParentId != null &&
            s.Id >= 10 &&
            s.ParentCode.EndsWith("hell");
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        Assert.That(@"(((m.parent_id IS NOT NULL) AND (m.id >= @Id)) AND (m.parent_code LIKE '%' + @ParentCode))", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(2));
        Assert.That(parametes, Contains.Key("Id"));
        Assert.That(parametes, Contains.Key("ParentCode"));

        Assert.That(parametes["Id"], Is.EqualTo(10));
        Assert.That(parametes["ParentCode"], Is.EqualTo("hell"));
    }

    [Test]
    public void GivenIdRange_ResultIsIdInRangeParameter()
    {
        var idCollection = new int[] { 1, 2, 3 };
        Expression<Func<TestModel, bool>> expression = s => idCollection.Contains(s.Id);
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        Assert.That(@"(m.id IN @IdCollection)", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(1));
        Assert.That(parametes, Contains.Key("IdCollection"));

        Assert.That(parametes["IdCollection"], Is.EqualTo(idCollection));
    }

    [Test]
    public void GivenOptionalName_ResultIsNameLikeParameterOrParameterIsNull()
    {
        string optionalName = "someName";
        int? idVal = null;
        Expression<Func<TestModel, bool>> expression = s => s.Name.LikeOrNull(optionalName) && s.Id.EqualsOrNull(idVal);
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        Assert.That(@"(((m.name LIKE '%' + @Name + '%') OR (@Name IS NULL)) AND ((m.id = @Id) OR (@Id IS NULL)))", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(2));
        Assert.That(parametes, Contains.Key("Id"));
        Assert.That(parametes, Contains.Key("Name"));

        Assert.That(parametes["Id"], Is.EqualTo(idVal));
        Assert.That(parametes["Name"], Is.EqualTo(optionalName));
    }

    [Test]
    public void GivenIdRange_ResultIsIdInRangeParameterOrParameterIsNull()
    {
        var idCollection = new int[] { 1, 2, 3 };
        Expression<Func<TestModel, bool>> expression = s => idCollection.ContainsOrNull(s.Id);
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        Assert.That(@"(m.id IN @IdCollection)", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(1));
        Assert.That(parametes, Contains.Key("IdCollection"));

        Assert.That(parametes["IdCollection"], Is.EqualTo(idCollection));
    }

    [Test]
    public void GivenIdRangeIsEmpty_ResultIsOneEqualsOneExpression()
    {
        var idCollection = new int[] { };
        Expression<Func<TestModel, bool>> expression = s => idCollection.ContainsOrNull(s.Id);
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        Assert.That(@"(1 = 1)", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(0));
    }

    [Test]
    public void GivenIdRangeIsNull_ResultIsOneEqualsOneExpression()
    {
        int[] idCollection = null;
        Expression<Func<TestModel, bool>> expression = s => idCollection.ContainsOrNull(s.Id);
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        Assert.That(@"(1 = 1)", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(0));
    }

    [Test]
    public void GivenStringForName_ResultIsNameLikeParam()
    {
        var name = "";
        Expression<Func<TestModel, bool>> expression = s => s.Name.Contains(name);
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        Assert.That(@"(m.name LIKE '%' + @Name + '%')", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(1));
        Assert.That(parametes, Contains.Key("Name"));

        Assert.That(parametes["Name"], Is.EqualTo(name));
    }

    [Test]
    public void GivenStringForName_ResultIsNameLikeParamOrParamIsNull()
    {
        var name = "";
        Expression<Func<TestModel, bool>> expression = s => s.Name.ContainsOrNull(name);
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        Assert.That(@"((m.name LIKE '%' + @Name + '%') OR (@Name IS NULL))", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(1));
        Assert.That(parametes, Contains.Key("Name"));

        Assert.That(parametes["Name"], Is.EqualTo(name));
    }

    [Test]
    public void GivenNameForSubmodelSimple_ResultIsSubModelNameLikeParamOrParamIsNull()
    {
        var name = "";
        Expression<Func<TestModel, bool>> expression = s => s.SubModel.Name.ContainsOrNull(name);
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        //SubModel.Name => s.name

        Assert.That(@"((s.name LIKE '%' + @SubModelName + '%') OR (@SubModelName IS NULL))", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(1));
        Assert.That(parametes, Contains.Key("SubModelName"));

        Assert.That(parametes["SubModelName"], Is.EqualTo(name));
    }

    [Test]
    public void GivenNameForSubmodel_ResultIsSubModelNameLikeParamOrParamIsNull()
    {
        var name = "";
        Expression<Func<TestModel, bool>> expression = s => s.Name.ContainsOrNull(name) || s.SubModel.Name.ContainsOrNull(name);
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        //SubModel.Name => s.name

        Assert.That(@"(((m.name LIKE '%' + @Name + '%') OR (@Name IS NULL)) OR ((s.name LIKE '%' + @SubModelName + '%') OR (@SubModelName IS NULL)))", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(2));
        Assert.That(parametes, Contains.Key("Name"));
        Assert.That(parametes, Contains.Key("SubModelName"));

        Assert.That(parametes["Name"], Is.EqualTo(name));
        Assert.That(parametes["SubModelName"], Is.EqualTo(name));
    }

    [Test]
    public void GivenNameContainsOrNullэто_для_те_ResultIsValidWhereExpression()
    {
        var name = "это для те";
        Expression<Func<TestModel, bool>> expression = s => s.Name.ContainsOrNull("это для те");
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        //SubModel.Name => s.name

        Assert.That(@"((m.name LIKE '%' + @Name + '%') OR (@Name IS NULL))", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(1));
        Assert.That(parametes, Contains.Key("Name"));

        Assert.That(parametes["Name"], Is.EqualTo(name));
    }


    [Test]
    public void GivenNam_StartsWith_тип_ResultIsValidWhereExpression()
    {
        var name = "тип";
        Expression<Func<TestModel, bool>> expression = s => s.Name.StartsWith(name);
        var parser = Parser.GetParser(expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        //SubModel.Name => s.name

        Assert.That(@"(m.name LIKE @Name + '%')", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(1));
        Assert.That(parametes, Contains.Key("Name"));

        Assert.That(parametes["Name"], Is.EqualTo(name));
    }

    [Test]
    public void GivenGetOrganizationSpecification_ResultIsValidWhereExpression()
    {
        var query = new TestQueryModel();
        var idCollection = new[] { 1167, 1216 };
        var spec = new GetTestModelSpecification(query, idCollection);
        var parser = Parser.GetParser(spec.Expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        //SubModel.Name => s.name

        Assert.That(@"((((m.name LIKE '%' + @Name + '%') OR (@Name IS NULL)) AND ((m.parent_id = @ParentId) OR (@ParentId IS NULL))) AND (m.id IN @IdCollection))", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(3));
        Assert.That(parametes, Contains.Key("Name"));
        Assert.That(parametes, Contains.Key("ParentId"));
        Assert.That(parametes, Contains.Key("IdCollection"));

        Assert.That(parametes["Name"], Is.EqualTo(query.Name), "Параметр Name не соответсвует значениею query.Name");
        Assert.That(parametes["ParentId"], Is.EqualTo(query.ParentId));
        Assert.That(parametes["IdCollection"], Is.EqualTo(idCollection));
    }

    [Test]
    public void GivenGetOrganizationSpecification2_ResultIsValidWhereExpression()
    {
        var query = new TestQueryModel();
        var idCollection = new int[] { };
        var spec = new GetTestModelSpecification(query, idCollection);
        var parser = Parser.GetParser(spec.Expression);
        var node = parser.Parse();
        var result = Parser.CreateResult(node, mapping);

        //SubModel.Name => s.name

        Assert.That(@"((((m.name LIKE '%' + @Name + '%') OR (@Name IS NULL)) AND ((m.parent_id = @ParentId) OR (@ParentId IS NULL))) AND (1 = 1))", Is.EqualTo(result.ResultExpression));

        var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

        Assert.That(parametes.Count, Is.EqualTo(2));
        Assert.That(parametes, Contains.Key("Name"));
        Assert.That(parametes, Contains.Key("ParentId"));

        Assert.That(parametes["Name"], Is.EqualTo(query.Name), "Параметр Name не соответсвует значениею query.Name");
        Assert.That(parametes["ParentId"], Is.EqualTo(query.ParentId));
    }
}

class TestQueryModel
{
    public string Name { get; set; }

    public int? ParentId { get; set; }

    public TestQueryModel()
    {

    }
}

class GetTestModelSpecification
{
    public GetTestModelSpecification(
           TestQueryModel query
        , int[] filterByIds)
        : this(x => x.Name.LikeOrNull(query.Name)
        && x.ParentId.EqualsOrNull(query.ParentId)
        && filterByIds.ContainsOrNull(x.Id))
    {
        /*
         * (((x.name LIKE @Name) OR (@Name IS NULL)) AND ((x.parent_id = @ParentId) OR (@ParentId IS NULL)) AND (x.Id IN @IdCollection) OR (@IdCollection IS NULL)))
         */
    }

    public GetTestModelSpecification(Expression<Func<Domain.TestModel, bool>> expression)
    {
        Expression = expression;
    }

    public Expression<Func<Domain.TestModel, bool>> Expression { get; set; }
}