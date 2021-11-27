using AutoFixture.Xunit2;
using MetalCore.CQS.Validation;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Validation
{
    public class ValidatorBaseTests
    {
        public abstract class ValidatorBaseStub : ValidatorBase<string>
        {
            public void AddRulePublic(Func<IEnumerable<BrokenRule>> rule)
            {
                AddRule(rule);
            }

            public void AddRulePublic(ThreadRuleRunTypeEnum threadRule, Func<IEnumerable<BrokenRule>> rule)
            {
                AddRule(threadRule, rule);
            }

            public void AddRulePublic(Func<Task<IEnumerable<BrokenRule>>> rule)
            {
                AddRule(rule);
            }

            public void AddRulePublic(ThreadRuleRunTypeEnum threadRule, Func<Task<IEnumerable<BrokenRule>>> rule)
            {
                AddRule(threadRule, rule);
            }

            public void AddRulesPublic(params Func<IEnumerable<BrokenRule>>[] rules)
            {
                AddRules(rules);
            }

            public void AddRulesPublic(ThreadRuleRunTypeEnum threadRule, params Func<IEnumerable<BrokenRule>>[] rules)
            {
                AddRules(threadRule, rules);
            }

            public void AddRulesPublic(params Func<Task<IEnumerable<BrokenRule>>>[] rules)
            {
                AddRules(rules);
            }

            public void AddRulesPublic(ThreadRuleRunTypeEnum threadRule, params Func<Task<IEnumerable<BrokenRule>>>[] rules)
            {
                AddRules(threadRule, rules);
            }

            public void AddRulesPublic(ICollection<Func<Task<IEnumerable<BrokenRule>>>> rules)
            {
                AddRules(rules);
            }

            public void AddRulesPublic(ThreadRuleRunTypeEnum threadRule, ICollection<Func<Task<IEnumerable<BrokenRule>>>> rules)
            {
                AddRules(threadRule, rules);
            }

            public Task<ICollection<BrokenRule>> ValidateRulesPublicAsync()
            {
                return ValidateRulesAsync();
            }
        }

        public class AddRule_Func_BrokenRule
        {
            [Fact]
            public async Task IfNullRuleAdded_ThenNoBrokenRulesReturned()
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(() => (IEnumerable<BrokenRule>)null);

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Empty(result);
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenMatchingBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(() => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(brokenRule, result.First());
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(() => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfSame2InvalidRulesAdded_Then1BrokenRulesReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>()
                {
                    CallBase = true
                };
                target.Object.AddRulePublic(() => new[] { brokenRule });
                target.Object.AddRulePublic(() => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfDifferent2InvalidRulesAdded_Then2BrokenRulesReturned(BrokenRule brokenRule, BrokenRule brokenRule2)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(() => new[] { brokenRule });
                target.Object.AddRulePublic(() => new[] { brokenRule2 });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(2, result.Count);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAdded_ThenRunOnSameThread(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulePublic(rule);
                target.Object.AddRulePublic(rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 2);
            }
        }

        public class AddRule_ThreadRuleRunType_Func_BrokenRule
        {
            [Fact]
            public async Task IfNullRuleAdded_ThenNoBrokenRulesReturned()
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, () => (IEnumerable<BrokenRule>)null);

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Empty(result);
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenMatchingBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, () => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(brokenRule, result.First());
            }

            [Theory, AutoData]
            public void IfInvalidRuleAddedAndUndefined_ThenNotImplementedExceptionThrown(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();

                // Act
                Action act = () => target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Undefined, () => new[] { brokenRule });

                // Assert
                Assert.Throws<NotImplementedException>(act);
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, () => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfSame2InvalidRulesAdded_Then1BrokenRulesReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, () => new[] { brokenRule });
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, () => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfDifferent2InvalidRulesAdded_Then2BrokenRulesReturned(BrokenRule brokenRule, BrokenRule brokenRule2)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, () => new[] { brokenRule });
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, () => new[] { brokenRule2 });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(2, result.Count);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAdded_ThenRunOnSameThread(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 2);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAddedAndSingle_ThenRunOnDifferentThreads(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Single, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Single, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAddedAndMultiple_ThenRunOnDifferentThreads(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }
        }

        public class AddRule_Func_Task_BrokenRule
        {
            [Fact]
            public async Task IfNullRuleAdded_ThenNoBrokenRulesReturned()
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(async () => await Task.FromResult((IEnumerable<BrokenRule>)null));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Empty(result);
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenMatchingBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(async () => await Task.FromResult(new[] { brokenRule }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(brokenRule, result.First());
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(async () => await Task.FromResult(new[] { brokenRule }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfSame2InvalidRulesAdded_Then1BrokenRulesReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(async () => await Task.FromResult(new[] { brokenRule }));
                target.Object.AddRulePublic(async () => await Task.FromResult(new[] { brokenRule }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfDifferent2InvalidRulesAdded_Then2BrokenRulesReturned(BrokenRule brokenRule, BrokenRule brokenRule2)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(async () => await Task.FromResult(new[] { brokenRule }));
                target.Object.AddRulePublic(async () => await Task.FromResult(new[] { brokenRule2 }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(2, result.Count);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAdded_ThenRunOnDifferentThreads(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<Task<IEnumerable<BrokenRule>>> rule = async () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return await Task.FromResult(new[] { brokenRule });
                };
                target.Object.AddRulePublic(rule);
                target.Object.AddRulePublic(rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }
        }

        public class AddRule_ThreadRuleRunType_Func_Task_BrokenRule
        {
            [Fact]
            public async Task IfNullRuleAdded_ThenNoBrokenRulesReturned()
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult((IEnumerable<BrokenRule>)null));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Empty(result);
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenMatchingBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult(new[] { brokenRule }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(brokenRule, result.First());
            }

            [Theory, AutoData]
            public void IfInvalidRuleAddedAndUndefined_ThenNotImplementedExceptionThrown(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();

                // Act
                Action act = () => target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Undefined, async () => await Task.FromResult(new[] { brokenRule }));

                // Assert
                Assert.Throws<NotImplementedException>(act);
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult(new[] { brokenRule }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfSame2InvalidRulesAdded_Then1BrokenRulesReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult(new[] { brokenRule }));
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult(new[] { brokenRule }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfDifferent2InvalidRulesAdded_Then2BrokenRulesReturned(BrokenRule brokenRule, BrokenRule brokenRule2)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult(new[] { brokenRule }));
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult(new[] { brokenRule2 }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(2, result.Count);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAdded_ThenRunOnDifferentThreads(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<Task<IEnumerable<BrokenRule>>> rule = async () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return await Task.FromResult(new[] { brokenRule });
                };
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAddedAndSingle_ThenRunOnDifferentThreads(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<Task<IEnumerable<BrokenRule>>> rule = async () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return await Task.FromResult(new[] { brokenRule });
                };
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Single, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Single, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAddedAndMultiple_ThenRunOnDifferentThreads(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<Task<IEnumerable<BrokenRule>>> rule = async () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return await Task.FromResult(new[] { brokenRule });
                };
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAddedAndShared_ThenRunOnSameThread(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<Task<IEnumerable<BrokenRule>>> rule = async () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return await Task.FromResult(new[] { brokenRule });
                };
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 2);
            }
        }

        public class AddRule_ThreadRuleRunTypeEnum_Func_BrokenRule
        {
            public static IEnumerable<object[]> SingleBrokenRuleTests => new[] {
                new object[] { ThreadRuleRunTypeEnum.Multiple, new BrokenRule("A", "B") },
                new object[] { ThreadRuleRunTypeEnum.Shared, new BrokenRule("A", "B") },
                new object[] { ThreadRuleRunTypeEnum.Single, new BrokenRule("A", "B") }
            };

            public static IEnumerable<object[]> TwoBrokenRuleTests => new[] {
                new object[] { ThreadRuleRunTypeEnum.Multiple, new BrokenRule("A", "B"), new BrokenRule("C", "D") },
                new object[] { ThreadRuleRunTypeEnum.Shared, new BrokenRule("A", "B"), new BrokenRule("C", "D") },
                new object[] { ThreadRuleRunTypeEnum.Single, new BrokenRule("A", "B"), new BrokenRule("C", "D") }
            };


            [Theory]
            [InlineAutoData(ThreadRuleRunTypeEnum.Multiple)]
            [InlineAutoData(ThreadRuleRunTypeEnum.Shared)]
            [InlineAutoData(ThreadRuleRunTypeEnum.Single)]
            public async Task IfNullRuleAdded_ThenNoBrokenRulesReturned(ThreadRuleRunTypeEnum threadType)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(threadType, () => (IEnumerable<BrokenRule>)null);

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Empty(result);
            }

            [Theory]
            [MemberData(nameof(SingleBrokenRuleTests))]
            public async Task IfInvalidRuleAdded_ThenMatchingBrokenRuleReturned(ThreadRuleRunTypeEnum threadType, BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(threadType, () => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(brokenRule, result.First());
            }

            [Theory]
            [MemberData(nameof(SingleBrokenRuleTests))]
            public async Task IfInvalidRuleAdded_ThenBrokenRuleReturned(ThreadRuleRunTypeEnum threadType, BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(threadType, () => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory]
            [MemberData(nameof(SingleBrokenRuleTests))]
            public async Task IfSame2InvalidRulesAdded_Then1BrokenRulesReturned(ThreadRuleRunTypeEnum threadType, BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(threadType, () => new[] { brokenRule });
                target.Object.AddRulePublic(threadType, () => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory]
            [MemberData(nameof(TwoBrokenRuleTests))]
            public async Task IfDifferent2InvalidRulesAdded_Then2BrokenRulesReturned(ThreadRuleRunTypeEnum threadType, BrokenRule brokenRule, BrokenRule brokenRule2)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulePublic(threadType, () => new[] { brokenRule });
                target.Object.AddRulePublic(threadType, () => new[] { brokenRule2 });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(2, result.Count);
            }

            [Theory, AutoData]
            public async Task If3InvalidRulesAdded_Multiple_ThenAllRunOnMultipleThread(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Theory, AutoData]
            public async Task If3InvalidRulesAdded_Shared_ThenRunOnOneThreadTotal(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 3);
            }

            [Theory, AutoData]
            public async Task If3InvalidRulesAdded_Single_ThenEachRunOnOwnThread(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Single, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Single, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Single, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Theory, AutoData]
            public async Task If1InvalidRulesAdded_MultipleAndShared_ThenEachRunOnOneThread(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Multiple, rule);
                target.Object.AddRulePublic(ThreadRuleRunTypeEnum.Shared, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

        }

        public class AddRules_Func_BrokenRule
        {
            [Fact]
            public async Task IfNullRuleAdded_ThenNoBrokenRulesReturned()
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(() => (IEnumerable<BrokenRule>)null);

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Empty(result);
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenMatchingBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(() => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(brokenRule, result.First());
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(() => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfSame2InvalidRulesAdded_Then1BrokenRulesReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(() => new[] { brokenRule });
                target.Object.AddRulesPublic(() => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfSame4InvalidRulesAdded_Then1BrokenRulesReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(() => new[] { brokenRule }, () => new[] { brokenRule });
                target.Object.AddRulesPublic(() => new[] { brokenRule }, () => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfDifferent2InvalidRulesAdded_Then2BrokenRulesReturned(BrokenRule brokenRule, BrokenRule brokenRule2)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(() => new[] { brokenRule });
                target.Object.AddRulesPublic(() => new[] { brokenRule2 });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(2, result.Count);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAdded_ThenRunOnSameThread(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulesPublic(rule);
                target.Object.AddRulesPublic(rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 2);
            }
        }

        public class AddRules_ThreadRuleRunType_Func_BrokenRule
        {
            [Fact]
            public async Task IfNullRuleAdded_ThenNoBrokenRulesReturned()
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, () => (IEnumerable<BrokenRule>)null);

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Empty(result);
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenMatchingBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, () => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(brokenRule, result.First());
            }

            [Theory, AutoData]
            public void IfInvalidRuleAddedAndUndefined_ThenNotImplementedExceptionThrown(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();

                // Act
                Action act = () => target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Undefined, () => new[] { brokenRule });

                // Assert
                Assert.Throws<NotImplementedException>(act);
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, () => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfSame2InvalidRulesAdded_Then1BrokenRulesReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, () => new[] { brokenRule });
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, () => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfDifferent2InvalidRulesAdded_Then2BrokenRulesReturned(BrokenRule brokenRule, BrokenRule brokenRule2)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, () => new[] { brokenRule });
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, () => new[] { brokenRule2 });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(2, result.Count);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAdded_ThenRunOnSameThread(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 2);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAddedAndSingle_ThenRunOnDifferentThreads(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Single, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Single, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAddedAndMultiple_ThenRunOnDifferentThreads(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }
        }

        public class AddRules_Func_Task_BrokenRule
        {
            [Fact]
            public async Task IfNullRuleAdded_ThenNoBrokenRulesReturned()
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(async () => await Task.FromResult((IEnumerable<BrokenRule>)null));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Empty(result);
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenMatchingBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(async () => await Task.FromResult(new[] { brokenRule }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(brokenRule, result.First());
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(async () => await Task.FromResult(new[] { brokenRule }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfSame2InvalidRulesAdded_Then1BrokenRulesReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(async () => await Task.FromResult(new[] { brokenRule }));
                target.Object.AddRulesPublic(async () => await Task.FromResult(new[] { brokenRule }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfSame2InvalidRulesAddedAsCollection_Then1BrokenRulesReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(new List<Func<Task<IEnumerable<BrokenRule>>>>
                {
                    async () => await Task.FromResult(new[] {brokenRule}),
                    async () => await Task.FromResult(new[] {brokenRule})
                });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfDifferent2InvalidRulesAdded_Then2BrokenRulesReturned(BrokenRule brokenRule, BrokenRule brokenRule2)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(async () => await Task.FromResult(new[] { brokenRule }));
                target.Object.AddRulesPublic(async () => await Task.FromResult(new[] { brokenRule2 }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(2, result.Count);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAdded_ThenRunOnSameThread(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<Task<IEnumerable<BrokenRule>>> rule = async () =>
                {
                    await Task.Delay(WAIT_TIME);
                    return await Task.FromResult(new[] { brokenRule });
                };
                target.Object.AddRulesPublic(rule);
                target.Object.AddRulesPublic(rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 2);
            }
        }

        public class AddRules_ThreadRuleRunType_Func_Task_BrokenRule
        {
            [Fact]
            public async Task IfNullRuleAdded_ThenNoBrokenRulesReturned()
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult((IEnumerable<BrokenRule>)null));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Empty(result);
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenMatchingBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult(new[] { brokenRule }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(brokenRule, result.First());
            }

            [Theory, AutoData]
            public void IfInvalidRuleAddedAndUndefined_ThenNotImplementedExceptionThrown(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();

                // Act
                Action act = () => target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Undefined, async () => await Task.FromResult(new[] { brokenRule }));

                // Assert
                Assert.Throws<NotImplementedException>(act);
            }

            [Theory, AutoData]
            public async Task IfInvalidRuleAdded_ThenBrokenRuleReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult(new[] { brokenRule }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfSame2InvalidRulesAdded_Then1BrokenRulesReturned(BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult(new[] { brokenRule }));
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult(new[] { brokenRule }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory, AutoData]
            public async Task IfDifferent2InvalidRulesAdded_Then2BrokenRulesReturned(BrokenRule brokenRule, BrokenRule brokenRule2)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult(new[] { brokenRule }));
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, async () => await Task.FromResult(new[] { brokenRule2 }));

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(2, result.Count);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAdded_ThenRunOnDifferentThreads(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<Task<IEnumerable<BrokenRule>>> rule = async () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return await Task.FromResult(new[] { brokenRule });
                };
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAddedAndSingle_ThenRunOnDifferentThreads(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<Task<IEnumerable<BrokenRule>>> rule = async () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return await Task.FromResult(new[] { brokenRule });
                };
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Single, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Single, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAddedAndMultiple_ThenRunOnDifferentThreads(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<Task<IEnumerable<BrokenRule>>> rule = async () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return await Task.FromResult(new[] { brokenRule });
                };
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Theory, AutoData]
            public async Task If2InvalidRulesAddedAndShared_ThenRunOnSameThread(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<Task<IEnumerable<BrokenRule>>> rule = async () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return await Task.FromResult(new[] { brokenRule });
                };
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 2);
            }
        }

        public class AddRules_ThreadRuleRunTypeEnum_Func_BrokenRule
        {
            public static IEnumerable<object[]> SingleBrokenRuleTests => new[] {
                new object[] { ThreadRuleRunTypeEnum.Multiple, new BrokenRule("A", "B") },
                new object[] { ThreadRuleRunTypeEnum.Shared, new BrokenRule("A", "B") },
                new object[] { ThreadRuleRunTypeEnum.Single, new BrokenRule("A", "B") }
            };

            public static IEnumerable<object[]> TwoBrokenRuleTests => new[] {
                new object[] { ThreadRuleRunTypeEnum.Multiple, new BrokenRule("A", "B"), new BrokenRule("C", "D") },
                new object[] { ThreadRuleRunTypeEnum.Shared, new BrokenRule("A", "B"), new BrokenRule("C", "D") },
                new object[] { ThreadRuleRunTypeEnum.Single, new BrokenRule("A", "B"), new BrokenRule("C", "D") }
            };


            [Theory]
            [InlineAutoData(ThreadRuleRunTypeEnum.Multiple)]
            [InlineAutoData(ThreadRuleRunTypeEnum.Shared)]
            [InlineAutoData(ThreadRuleRunTypeEnum.Single)]
            public async Task IfNullRuleAdded_ThenNoBrokenRulesReturned(ThreadRuleRunTypeEnum threadType)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(threadType, () => (IEnumerable<BrokenRule>)null);

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Empty(result);
            }

            [Theory]
            [MemberData(nameof(SingleBrokenRuleTests))]
            public async Task IfInvalidRuleAdded_ThenMatchingBrokenRuleReturned(ThreadRuleRunTypeEnum threadType, BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(threadType, () => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(brokenRule, result.First());
            }

            [Theory]
            [MemberData(nameof(SingleBrokenRuleTests))]
            public async Task IfInvalidRuleAdded_ThenBrokenRuleReturned(ThreadRuleRunTypeEnum threadType, BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(threadType, () => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory]
            [MemberData(nameof(SingleBrokenRuleTests))]
            public async Task IfSame2InvalidRulesAdded_Then1BrokenRulesReturned(ThreadRuleRunTypeEnum threadType, BrokenRule brokenRule)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(threadType, () => new[] { brokenRule });
                target.Object.AddRulesPublic(threadType, () => new[] { brokenRule });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Single(result);
            }

            [Theory]
            [MemberData(nameof(TwoBrokenRuleTests))]
            public async Task IfDifferent2InvalidRulesAdded_Then2BrokenRulesReturned(ThreadRuleRunTypeEnum threadType, BrokenRule brokenRule, BrokenRule brokenRule2)
            {
                // Arrange
                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                target.Object.AddRulesPublic(threadType, () => new[] { brokenRule });
                target.Object.AddRulesPublic(threadType, () => new[] { brokenRule2 });

                // Act
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();

                // Assert
                Assert.Equal(2, result.Count);
            }

            [Theory, AutoData]
            public async Task If3InvalidRulesAdded_Multiple_ThenAllRunOnMultipleThread(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Theory, AutoData]
            public async Task If3InvalidRulesAdded_Shared_ThenRunOnOneThreadTotal(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 3);
            }

            [Theory, AutoData]
            public async Task If3InvalidRulesAdded_Single_ThenEachRunOnOwnThread(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Single, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Single, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Single, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Theory, AutoData]
            public async Task If1InvalidRulesAdded_MultipleAndShared_ThenEachRunOnOneThread(BrokenRule brokenRule)
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<ValidatorBaseStub> target = new Mock<ValidatorBaseStub>();
                Func<IEnumerable<BrokenRule>> rule = () =>
                {
                    Thread.Sleep(WAIT_TIME);
                    return new[] { brokenRule };
                };
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Multiple, rule);
                target.Object.AddRulesPublic(ThreadRuleRunTypeEnum.Shared, rule);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                ICollection<BrokenRule> result = await target.Object.ValidateRulesPublicAsync();
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }
        }
    }
}