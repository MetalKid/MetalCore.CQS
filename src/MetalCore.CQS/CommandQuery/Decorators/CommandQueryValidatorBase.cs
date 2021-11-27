using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This class handles running all the rules (sync and async) at the same time on multiple threads.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of command query being validated.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public abstract class CommandQueryValidatorBase<TCommandQuery, TResult> : ValidatorBase<TCommandQuery>,
        ICommandQueryValidator<TCommandQuery, TResult> where TCommandQuery : ICommandQuery<TResult>
    {
        /// <summary>
        /// Gets the command query being validated.
        /// </summary>
        protected TCommandQuery CommandQuery { get; private set; }

        /// <summary>
        /// Ensures that the command query is valid.
        /// </summary>
        /// <param name="commandQuery">The command query to validate.</param>
        /// <param name="validate">The validation helper.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>Any broken rules that were found.</returns>
        public async Task<IEnumerable<BrokenRule>> ValidateCommandQueryAsync(
            TCommandQuery commandQuery,
            CancellationToken token = default)
        {
            Guard.IsNotNull(commandQuery, nameof(commandQuery));

            CommandQuery = commandQuery;

            CreateRules(commandQuery, token);

            return await ValidateRulesAsync().ConfigureAwait(false);
        }
    }
}