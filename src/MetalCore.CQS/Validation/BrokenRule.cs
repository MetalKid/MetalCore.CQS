using System;

namespace MetalCore.CQS.Validation
{
    /// <summary>
    /// This class stores information about a rule that has been broken.
    /// </summary>
    public class BrokenRule : IEquatable<BrokenRule>
    {
        /// <summary>
        /// Gets or sets the message to display for this exception.
        /// </summary>
        public string RuleMessage { get; set; }

        /// <summary>
        /// Gets or sets a specific reference for this error. (Could be used by UI.)
        /// </summary>
        public string Relation { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BrokenRule() { }

        /// <summary>
        /// Constructor that defaults the message and relation fields.
        /// </summary>
        /// <param name="ruleMessage">The broken rule message.</param>_
        /// <param name="relation">Defines a specific reference for this rule.</param>
        public BrokenRule(string ruleMessage, string relation = "")
        {
            Guard.IsNotNullOrWhiteSpace(ruleMessage, nameof(ruleMessage));

            RuleMessage = ruleMessage;
            Relation = relation;
        }

        /// <summary>
        /// Returns whether this and another BrokenRule object are equivalent.
        /// </summary>
        /// <param name="other">The other BrokenRule to inspect.</param>
        /// <returns>True if they are equal; false otherwise.</returns>
        public bool Equals(BrokenRule other) =>
            !(other is null) &&
            (ReferenceEquals(this, other) ||
            (Equals(RuleMessage, other.RuleMessage) && Equals(Relation, other.Relation)));

        /// <summary>
        /// Returns whether this and another instance object are equivalent.
        /// </summary>
        /// <param name="other">The other instance to inspect.</param>
        /// <returns>True if they are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is BrokenRule other)
            {
                return Equals(other);
            }

            return false;
        }

        /// <summary>
        /// Returns the HashCode value of this class for uniqueness.
        /// </summary>
        /// <returns>HashCode of class.</returns>
        public override int GetHashCode()
        {
            return RuleMessage.GetHashCode() ^ (Relation?.GetHashCode() ?? 0);
        }
    }
}