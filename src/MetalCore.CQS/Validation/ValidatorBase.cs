using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Validation
{
    /// <summary>
    /// This base class handles running all the validation rules on the appropriate thread(s).
    /// </summary>
    /// <typeparam name="T">The type of item to run rules against.</typeparam>
    public abstract class ValidatorBase<T>
    {
        // First item in list is the Shared thread
        private readonly IList<ICollection<Func<Task<IEnumerable<BrokenRule>>>>> _threadRules =
            new List<ICollection<Func<Task<IEnumerable<BrokenRule>>>>>() { new List<Func<Task<IEnumerable<BrokenRule>>>>() };

        /// <summary>
        /// Create the rules to run.
        /// </summary>
        /// <param name="input">The item to run rules against.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        protected abstract void CreateRules(T input, CancellationToken token = default);

        /// <summary>
        /// Adds a synchronous rule to run.
        /// </summary>
        /// <param name="rule">The sync rule to run.</param>
        /// <remarks>Task.FromResult has very little overhead.</remarks>
        protected void AddRule(Func<IEnumerable<BrokenRule>> rule)
        {
            AddRules(ThreadRuleRunTypeEnum.Shared, rule);
        }

        /// <summary>
        /// Adds a synchronous rule to run.
        /// </summary>
        /// <param name="threadRule">The thread strategy to run the given rule under.</param>
        /// <param name="rule">The sync rule to run.</param>
        /// <remarks>Task.FromResult has very little overhead.</remarks>
        protected void AddRule(ThreadRuleRunTypeEnum threadRule, Func<IEnumerable<BrokenRule>> rule)
        {
            AddRules(threadRule, rule);
        }

        /// <summary>
        /// Adds an asynchronous rule to run.
        /// </summary>
        /// <param name="rule">The async rule to run.</param>
        protected void AddRule(Func<Task<IEnumerable<BrokenRule>>> rule)
        {
            AddRules(ThreadRuleRunTypeEnum.Multiple, rule);
        }

        /// <summary>
        /// Adds an asynchronous rule to run.
        /// </summary>
        /// <param name="threadRule">The thread strategy to run the given rule under.</param>
        /// <param name="rule">The async rule to run.</param>
        protected void AddRule(ThreadRuleRunTypeEnum threadRule, Func<Task<IEnumerable<BrokenRule>>> rule)
        {
            AddRules(threadRule, rule);
        }

        /// <summary>
        /// Adds synchronous rules to run.
        /// </summary>
        /// <param name="rules">The sync rules to run.</param>
        /// <remarks>Task.FromResult has very little overhead.</remarks>
        protected void AddRules(params Func<IEnumerable<BrokenRule>>[] rules)
        {
            AddRules(ThreadRuleRunTypeEnum.Shared, rules);
        }

        /// <summary>
        /// Adds synchronous rules to run.
        /// </summary>
        /// <param name="threadRule">The thread strategy to run the given rules under.</param>
        /// <param name="rules">The sync rules to run.</param>
        /// <remarks>Task.FromResult has very little overhead.</remarks>
        protected void AddRules(ThreadRuleRunTypeEnum threadRule, params Func<IEnumerable<BrokenRule>>[] rules)
        {
            List<Func<Task<IEnumerable<BrokenRule>>>> list = new List<Func<Task<IEnumerable<BrokenRule>>>>();
            foreach (Func<IEnumerable<BrokenRule>> rule in rules)
            {
                list.Add(() => Task.FromResult(rule()));
            }
            AddRules(threadRule, list.ToArray());
        }

        /// <summary>
        /// Adds asynchronous rules to run.
        /// </summary>
        /// <param name="rules">The async rule to run.</param>
        protected void AddRules(params Func<Task<IEnumerable<BrokenRule>>>[] rules)
        {
            AddRules(ThreadRuleRunTypeEnum.Shared, rules);
        }

        /// <summary>
        /// Adds asynchronous rules to run.
        /// </summary>
        /// <param name="threadRule">The thread strategy to run the given rule under.</param>
        /// <param name="rules">The async rule to run.</param>
        protected void AddRules(ThreadRuleRunTypeEnum threadRule, params Func<Task<IEnumerable<BrokenRule>>>[] rules)
        {
            AddRules(threadRule, (ICollection<Func<Task<IEnumerable<BrokenRule>>>>)rules);
        }

        /// <summary>
        /// Adds asynchronous rules to run.
        /// </summary>
        /// <param name="rules">The async rule to run.</param>
        protected void AddRules(ICollection<Func<Task<IEnumerable<BrokenRule>>>> rules)
        {
            AddRules(ThreadRuleRunTypeEnum.Multiple, rules);
        }

        /// <summary>
        /// Adds asynchronous rules to run.
        /// </summary>
        /// <param name="threadRule">The thread strategy to run the given rule under.</param>
        /// <param name="rules">The async rule to run.</param>
        protected void AddRules(ThreadRuleRunTypeEnum threadRule, ICollection<Func<Task<IEnumerable<BrokenRule>>>> rules)
        {
            switch (threadRule)
            {
                case ThreadRuleRunTypeEnum.Shared:
                    foreach (Func<Task<IEnumerable<BrokenRule>>> rule in rules)
                    {
                        _threadRules[0].Add(rule);
                    }
                    return;
                case ThreadRuleRunTypeEnum.Single:
                    _threadRules.Add(rules);
                    return;
                case ThreadRuleRunTypeEnum.Multiple:
                    foreach (Func<Task<IEnumerable<BrokenRule>>> rule in rules)
                    {
                        _threadRules.Add(new List<Func<Task<IEnumerable<BrokenRule>>>> { rule });
                    }
                    return;
                default:
                    throw new NotImplementedException($"ThreadRuleRunTypeEnum '{threadRule}' is not implemented.");
            }
        }

        /// <summary>
        /// Runs all validation rules and returns any broken rules.
        /// </summary>
        /// <returns>Any broken validation rules.</returns>
        protected async Task<ICollection<BrokenRule>> ValidateRulesAsync()
        {
            List<Task<ICollection<IEnumerable<BrokenRule>>>> tasks = _threadRules.AsParallel().Select(CreateValidationThread).ToList();

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return tasks.SelectMany(a => a.Result).SelectMany(a => a).Distinct().ToList();
        }

        /// <summary>
        /// Returns a thread that runs all of the validations rules and returns any broken ones.
        /// </summary>
        /// <param name="rules">The rules to run.</param>
        /// <returns>Any broken rules.</returns>
        private async Task<ICollection<IEnumerable<BrokenRule>>> CreateValidationThread(ICollection<Func<Task<IEnumerable<BrokenRule>>>> rules)
        {
            List<IEnumerable<BrokenRule>> result = new List<IEnumerable<BrokenRule>>();

            foreach (Func<Task<IEnumerable<BrokenRule>>> rule in rules)
            {
                IEnumerable<BrokenRule> brokenRules = await rule().ConfigureAwait(false);
                if (brokenRules == null || !brokenRules.Any())
                {
                    continue;

                }
                result.Add(brokenRules);
            }

            return result;
        }
    }
}