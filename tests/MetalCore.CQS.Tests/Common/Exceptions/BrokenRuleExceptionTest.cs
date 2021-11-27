using AutoFixture.Xunit2;
using MetalCore.CQS.Exceptions;
using MetalCore.CQS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Exceptions
{
    public class BrokenRuleExceptionTest
    {
        public class ContstructorMethods
        {
            [Fact]
            public void IfDefault_Then1BrokenRuleWithNullProperties()
            {
                // Arrange

                // Act
                BrokenRuleException result = new BrokenRuleException();

                // Assert
                Assert.Equal(1, result.BrokenRules.Count);
                Assert.Null(result.BrokenRules.Single().RuleMessage);
                Assert.Null(result.BrokenRules.Single().Relation);
                Assert.NotNull(result.Message);
                Assert.Null(result.InnerException);
            }

            [Fact]
            public void IfDefaultWithDuplicateBrokenRule_Then1BrokenRuleWithNullProperties()
            {
                // Arrange
                BrokenRule rule1 = new BrokenRule();
                BrokenRule rule2 = new BrokenRule();

                // Act
                BrokenRuleException result = new BrokenRuleException(new List<BrokenRule>() { rule1, rule2 });

                // Assert
                Assert.Single(result.BrokenRules);
                Assert.Null(result.BrokenRules.Single().RuleMessage);
                Assert.Null(result.BrokenRules.Single().Relation);
                Assert.NotNull(result.Message);
                Assert.Null(result.InnerException);
            }

            [Theory]
            [AutoData]
            public void IfDuplicateBrokenRulesAnd3OtherUniqueRules_Then4BrokenRules(string message, string relation)
            {
                // Arrange
                BrokenRule rule1 = new BrokenRule(message, relation);
                BrokenRule rule2 = new BrokenRule(message, relation);
                BrokenRule rule3 = new BrokenRule(message);
                BrokenRule rule4 = new BrokenRule();
                BrokenRule rule5 = new BrokenRule() { Relation = relation };

                // Act
                BrokenRuleException result = new BrokenRuleException(new List<BrokenRule>() { rule1, rule2, rule3, rule4, rule5 });

                // Assert
                Assert.Equal(4, result.BrokenRules.Count);
            }

            [Fact]
            public void IfNullBrokenRule_Then1BrokenRuleWithNullProperties()
            {
                // Arrange
                BrokenRule rule = null;

                // Act
                BrokenRuleException result = new BrokenRuleException(rule);

                // Assert
                Assert.Equal(1, result.BrokenRules.Count);
                Assert.Null(result.BrokenRules.Single());
                Assert.Null(result.BrokenRules.Single());
                Assert.NotNull(result.Message);
                Assert.Null(result.InnerException);
            }

            [Fact]
            public void IfNonNullBrokenRule_Then1BrokenRuleWithNullProperties()
            {
                // Arrange
                BrokenRule rule = new BrokenRule();

                // Act
                BrokenRuleException result = new BrokenRuleException(rule);

                // Assert
                Assert.Equal(1, result.BrokenRules.Count);
                Assert.Null(result.BrokenRules.Single().RuleMessage);
                Assert.Null(result.BrokenRules.Single().Relation);
                Assert.NotNull(result.Message);
                Assert.Null(result.InnerException);
            }

            [Fact]
            public void IfFilledBrokenRule_Then1BrokenRuleWithFilledProperties()
            {
                // Arrange
                BrokenRule rule = new BrokenRule { Relation = "Test1", RuleMessage = "Test2" };

                // Act
                BrokenRuleException result = new BrokenRuleException(rule);

                // Assert
                Assert.Equal(1, result.BrokenRules.Count);
                Assert.Equal(rule.RuleMessage, result.BrokenRules.Single().RuleMessage);
                Assert.Equal(rule.Relation, result.BrokenRules.Single().Relation);
                Assert.NotNull(result.Message);
                Assert.Null(result.InnerException);
            }

            [Fact]
            public void IfInnerExceptionBrokenRule_Then1BrokenRuleInnerExceptionProperties()
            {
                // Arrange
                string message = "Test";
                Exception innerEx = new Exception(message);

                // Act
                BrokenRuleException result = new BrokenRuleException(message, innerEx);

                // Assert
                Assert.Equal(message, result.Message);
                Assert.Equal(innerEx, result.InnerException);
            }
        }
    }
}