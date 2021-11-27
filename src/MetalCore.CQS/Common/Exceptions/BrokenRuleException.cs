using MetalCore.CQS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetalCore.CQS.Exceptions
{
    /// <summary>
    /// This exception is thrown when rules are broken validating a command.
    /// </summary>
    public class BrokenRuleException : Exception
    {
        /// <summary>
        /// Gets or sets the list of broken rules that occurred.
        /// </summary>
        public ICollection<BrokenRule> BrokenRules { get; }

        /// <summary>
        /// Defalt constructor.
        /// </summary>
        public BrokenRuleException() : this(null, null)
        {
        }

        /// <summary>
        /// Constructor that takes a single Broken Rule.
        /// </summary>
        /// <param name="rule">The broken rule that occurred.</param>
        public BrokenRuleException(BrokenRule rule)
        {
            BrokenRules = new List<BrokenRule> { rule };
        }

        /// <summary>
        /// Constructor that takes a list of Broken Rules.
        /// </summary>
        /// <param name="rules">The broken rule(s) that occurred.</param>
        public BrokenRuleException(IEnumerable<BrokenRule> rules)
        {
            List<BrokenRule> list = new List<BrokenRule>();
            foreach (BrokenRule rule in rules)
            {
                //Ensure no duplicate rules
                if (list.Any(a => a.Relation == rule.Relation && a.RuleMessage == rule.RuleMessage))
                {
                    continue;
                }
                list.Add(rule);
            }
            BrokenRules = list;
        }

        /// <summary>
        /// Constructor that takes a message and creates a Broken Rule and inner exception.
        /// </summary>
        /// <param name="message">The message of the exception and the RuleMessage of the BrokenRule.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="relation">For the UI to use to put the error on the screen somewhow.</param>
        public BrokenRuleException(
            string message = null,
            Exception innerException = null,
            string relation = null) : base(message, innerException)
        {
            BrokenRules = new List<BrokenRule>
            {
                new BrokenRule { RuleMessage = message, Relation = relation }
            };
        }
    }
}