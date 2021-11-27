using AutoFixture.Xunit2;
using MetalCore.CQS.Validation;
using System;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Validation
{
    public class BrokenRuleTests
    {
        [Fact]
        public void IfNullMessage_ThenArgumentNullExceptionThrown()
        {
            // Arrange

            // Act
            Action act = () => new BrokenRule(null);

            // Assert
            Assert.Throws<ArgumentException>("ruleMessage", act);
        }

        [Fact]
        public void IfBlankMessage_ThenArgumentNullExceptionThrown()
        {
            // Arrange

            // Act
            Action act = () => new BrokenRule("");

            // Assert
            Assert.Throws<ArgumentException>("ruleMessage", act);
        }

        [Fact]
        public void IfWhitespaceMessage_ThenArgumentNullExceptionThrown()
        {
            // Arrange

            // Act
            Action act = () => new BrokenRule(" ");

            // Assert
            Assert.Throws<ArgumentException>("ruleMessage", act);
        }

        [Fact]
        public void IfNullRelation_ThenNoExceptionThrownAndNotNullInstance()
        {
            // Arrange

            // Act
            BrokenRule result = new BrokenRule("Test", null);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void IfBlankRelation_ThenNoExceptionThrownAndNotNullInstance()
        {
            // Arrange

            // Act
            BrokenRule result = new BrokenRule("Test", "");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void IfWhitespaceRelation_ThenNoExceptionThrownAndNotNullInstance()
        {
            // Arrange

            // Act
            BrokenRule result = new BrokenRule("Test", " ");

            // Assert
            Assert.NotNull(result);
        }
    }

    public class RuleMessageProperty
    {
        [Theory]
        [AutoData]
        public void IfNotNullOrBlankOrWhitespaceRuleMessage_ThenPropertyReturnsGivenValue(string ruleMessage)
        {
            // Arrange

            // Act
            BrokenRule result = new BrokenRule(ruleMessage);

            // Assert
            Assert.Equal(ruleMessage, result.RuleMessage);
        }
    }

    public class RelationProperty
    {
        [Theory]
        [AutoData]
        public void IfNullRelation_ThenPropertyReturnsNull(string ruleMessage)
        {
            // Arrange

            // Act
            BrokenRule result = new BrokenRule(ruleMessage, null);

            // Assert
            Assert.Null(result.Relation);
        }

        [Theory]
        [AutoData]
        public void IfNlankRelation_ThenPropertyReturnsBlank(string ruleMessage)
        {
            // Arrange
            string relation = "";

            // Act
            BrokenRule result = new BrokenRule(ruleMessage, relation);

            // Assert
            Assert.Equal(relation, result.Relation);
        }

        [Theory]
        [AutoData]
        public void IfNotNullOrBlankOrWhitespaceRelation_ThenPropertyReturnsGivenValue(string ruleMessage, string relation)
        {
            // Arrange

            // Act
            BrokenRule result = new BrokenRule(ruleMessage, relation);

            // Assert
            Assert.Equal(relation, result.Relation);
        }
    }

    public class Equals
    {
        [Theory]
        [AutoData]
        public void IfSameRuleMessage_ThenObjectsAreEqual(string ruleMessage)
        {
            // Arrange
            BrokenRule other = new BrokenRule(ruleMessage);

            // Act
            BrokenRule result = new BrokenRule(ruleMessage);

            // Assert
            Assert.Equal(other, result);
        }

        [Theory]
        [AutoData]
        public void IfDifferentRuleMessage_ThenObjectsAreNotEqual(string ruleMessage, string otherRuleMessage)
        {
            // Arrange
            BrokenRule other = new BrokenRule(otherRuleMessage);

            // Act
            BrokenRule result = new BrokenRule(ruleMessage);

            // Assert
            Assert.NotEqual(other, result);
        }

        [Theory]
        [AutoData]
        public void IfOtherIsNull_ThenObjectsAreNotEqual(string ruleMessage)
        {
            // Arrange
            BrokenRule other = null;

            // Act
            BrokenRule result = new BrokenRule(ruleMessage);

            // Assert
            Assert.NotEqual(other, result);
        }

        [Theory]
        [AutoData]
        public void IfSameRuleMessageAndSameRelation_ThenObjectsAreEqual(string ruleMessage, string relation)
        {
            // Arrange
            BrokenRule other = new BrokenRule(ruleMessage, relation);

            // Act
            BrokenRule result = new BrokenRule(ruleMessage, relation);

            // Assert
            Assert.Equal(other, result);
        }

        [Theory]
        [AutoData]
        public void IfSameObject_ThenObjectsAreEqual(string ruleMessage, string relation)
        {
            // Arrange

            // Act
            BrokenRule result = new BrokenRule(ruleMessage, relation);

            // Assert
            Assert.Equal(result, result);
        }
    }
}
