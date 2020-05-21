using System;
using System.Linq;
using System.Linq.Expressions;

using Domain;
using ExpressionParser.Parser;
using Infrastructure;

using NUnit.Framework;

namespace ExpressionParser
{
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
            var parser = Parser.ExpressionParser.GetParserForExpression(expression);
            var node = parser.Parse(mapping);
            var result = Parser.ExpressionParser.CreateResult(node);

            Assert.AreEqual("(((m.parent_id = @ParentId) AND (m.code = @Code)) OR (m.name = @Name))", result.ResultExpression);

            var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

            Assert.IsTrue(parametes.Count == 3);

            Assert.IsTrue(parametes.ContainsKey("ParentId"));
            Assert.IsTrue(parametes.ContainsKey("Code"));
            Assert.IsTrue(parametes.ContainsKey("Name"));

            Assert.AreEqual(parametes["ParentId"], 1);
            Assert.AreEqual(parametes["Code"], "CODEXX1");
            Assert.AreEqual(parametes["Name"], "zzz");
        }


        [Test]
        public void GivenParentIdWithCodeOrName_ResultWithExpectedParentesis()
        {
            Expression<Func<TestModel, bool>> expression = s => s.ParentId == 1 && (s.Code == "CODEXX1" || s.Name == "zzz");
            var parser = Parser.ExpressionParser.GetParserForExpression(expression);
            var node = parser.Parse(mapping);
            var result = Parser.ExpressionParser.CreateResult(node);

            Assert.AreEqual(@"((m.parent_id = @ParentId) AND ((m.code = @Code) OR (m.name = @Name)))", result.ResultExpression);

            var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

            Assert.IsTrue(parametes.Count == 3);

            Assert.IsTrue(parametes.ContainsKey("ParentId"));
            Assert.IsTrue(parametes.ContainsKey("Code"));
            Assert.IsTrue(parametes.ContainsKey("Name"));

            Assert.AreEqual(parametes["ParentId"], 1);
            Assert.AreEqual(parametes["Code"], "CODEXX1");
            Assert.AreEqual(parametes["Name"], "zzz");
        }

        [Test]
        public void GivenCodeNotEquealsCODEXX1_ResultNotEqualsTranslatedSuccess()
        {
            Expression<Func<TestModel, bool>> expression = s => s.Code != "CODEXX1";
            var parser = Parser.ExpressionParser.GetParserForExpression(expression);
            var node = parser.Parse(mapping);
            var result = Parser.ExpressionParser.CreateResult(node);

            Assert.AreEqual(@"(m.code != @Code)", result.ResultExpression);

            var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

            Assert.IsTrue(parametes.Count == 1);

            Assert.IsTrue(parametes.ContainsKey("Code"));

            Assert.AreEqual(parametes["Code"], "CODEXX1");
        }

        [Test]
        public void GivenParentIdIsNotNull_ResultIsParentIdIsNotNull()
        {
            Expression<Func<TestModel, bool>> expression = s => s.ParentId != null;
            var parser = Parser.ExpressionParser.GetParserForExpression(expression);
            var node = parser.Parse(mapping);
            var result = Parser.ExpressionParser.CreateResult(node);

            Assert.AreEqual(@"(m.parent_id IS NOT NULL)", result.ResultExpression);

            Assert.IsTrue(result.Parameters.Count == 0);
        }


        [Test]
        public void GivenParentIdIsNotNullAndIdGreater10_ResultIsParentIdIsNotNullAndIdGreaterIdParam()
        {
            Expression<Func<TestModel, bool>> expression = s => s.ParentId != null && s.Id > 10;
            var parser = Parser.ExpressionParser.GetParserForExpression(expression);
            var node = parser.Parse(mapping);
            var result = Parser.ExpressionParser.CreateResult(node);

            Assert.AreEqual(@"((m.parent_id IS NOT NULL) AND (m.id > @Id))", result.ResultExpression);

            var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

            Assert.IsTrue(parametes.Count == 1);

            Assert.IsTrue(parametes.ContainsKey("Id"));

            Assert.AreEqual(parametes["Id"], 10);
        }

        [Test]
        public void GivenParentIdIsNotNullAndIdGreater10AndParentCodeStartsWith_ResultIsParentIdIsNotNullAndIdGreaterIdParamAndParentCodeLikeParam()
        {
            Expression<Func<TestModel, bool>> expression = s =>
                s.ParentId != null &&
                s.Id >= 10 &&
                s.ParentCode.EndsWith("hell");
            var parser = Parser.ExpressionParser.GetParserForExpression(expression);
            var node = parser.Parse(mapping);
            var result = Parser.ExpressionParser.CreateResult(node);

            Assert.AreEqual(@"(((m.parent_id IS NOT NULL) AND (m.id >= @Id)) AND (m.parent_code LIKE ""%"" + @ParentCode))", result.ResultExpression);

            var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

            Assert.IsTrue(parametes.Count == 2);
            Assert.IsTrue(parametes.ContainsKey("Id"));
            Assert.IsTrue(parametes.ContainsKey("ParentCode"));

            Assert.AreEqual(parametes["Id"], 10);
            Assert.AreEqual(parametes["ParentCode"], "hell");
        }

        [Test]
        public void GivenIdRange_ResultIsIdInRangeParameter()
        {
            var idCollection = new int[] { 1, 2, 3 };
            Expression<Func<TestModel, bool>> expression = s => idCollection.Contains(s.Id);
            var parser = Parser.ExpressionParser.GetParserForExpression(expression);
            var node = parser.Parse(mapping);
            var result = Parser.ExpressionParser.CreateResult(node);

            Assert.AreEqual(@"(m.id IN @IdCollection)", result.ResultExpression);

            var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

            Assert.IsTrue(parametes.Count == 1);
            Assert.IsTrue(parametes.ContainsKey("IdCollection"));

            Assert.AreEqual(parametes["IdCollection"], idCollection);
        }

        [Test]
        public void GivenOptionalName_ResultIsNameLikeParameterOrParameterIsNull()
        {
            string optionalName = "someName";
            int? idVal = null;
            Expression<Func<TestModel, bool>> expression = s => s.Name.LikeOrNull(optionalName) && s.Id.EqualsOrNull(idVal);
            var parser = Parser.ExpressionParser.GetParserForExpression(expression);
            var node = parser.Parse(mapping);
            var result = Parser.ExpressionParser.CreateResult(node);

            Assert.AreEqual(@"(((m.name LIKE ""%"" + @Name + ""%"") OR (@Name IS NULL)) AND ((m.id = @Id) OR (@Id IS NULL)))", result.ResultExpression);

            var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

            Assert.IsTrue(parametes.Count == 2);
            Assert.IsTrue(parametes.ContainsKey("Id"));
            Assert.IsTrue(parametes.ContainsKey("Name"));

            Assert.AreEqual(parametes["Id"], idVal);
            Assert.AreEqual(parametes["Name"], optionalName);
        }

        [Test]
        public void GivenIdRange_ResultIsIdInRangeParameterOrParameterIsNull()
        {
            var idCollection = new int[] { 1, 2, 3 };
            Expression<Func<TestModel, bool>> expression = s => idCollection.ContainsOrNull(s.Id);
            var parser = Parser.ExpressionParser.GetParserForExpression(expression);
            var node = parser.Parse(mapping);
            var result = Parser.ExpressionParser.CreateResult(node);

            Assert.AreEqual(@"((m.id IN @IdCollection) OR (@IdCollection IS NULL))", result.ResultExpression);

            var parametes = result.Parameters.ToDictionary(x => x.Name, x => x.Value);

            Assert.IsTrue(parametes.Count == 1);
            Assert.IsTrue(parametes.ContainsKey("IdCollection"));

            Assert.AreEqual(parametes["IdCollection"], idCollection);
        }
    }
}