using MetalCore.CQS.Command;
using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Query;
using MetalCore.CQS.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Mediators
{
    /// <summary>
    /// This class handles calling the correct command, query, or command query handler automatically.
    /// </summary>
    public class CqsMediator : ICqsMediator
    {
        private readonly Func<Type, dynamic> _getInstanceCallback;

        /// <summary>
        /// Constructor that takes an anonymous function to lookup an instance from the outside.
        /// </summary>
        /// <param name="getInstanceCallback">The callback to get an instance of a dynamic type.</param>
        public CqsMediator(Func<Type, dynamic> getInstanceCallback)
        {
            Guard.IsNotNull(getInstanceCallback, nameof(getInstanceCallback));

            _getInstanceCallback = getInstanceCallback;
        }

        /// <summary>
        /// Executes the command handler for the given command automatically.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        public async Task<IResult> ExecuteAsync(
            ICommand command,
            CancellationToken token = default)
        {
            Guard.IsNotNull(command, nameof(command));

            await PreProcessAsync(command, token).ConfigureAwait(false);

            Type commandType = command.GetType();
            Type type = typeof(ICommandHandler<>).MakeGenericType(commandType);
            dynamic instance = _getInstanceCallback(type);
            if (instance == null)
            {
                throw new TypeLoadException(
                    $"No command handler type found for command type: {commandType.FullName}");
            }

            dynamic specificCommand = Convert.ChangeType(command, commandType);
            var result = await instance.ExecuteAsync(specificCommand, token).ConfigureAwait(false);
            await PostProcessAsync(command, result, token).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Executes the query handler for the given query automatically.
        /// </summary>
        /// <param name="commandQuery">The command query to execute.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the query.</returns>
        public async Task<IResult<TResult>> ExecuteAsync<TResult>(
            ICommandQuery<TResult> commandQuery,
            CancellationToken token = default)
        {
            Guard.IsNotNull(commandQuery, nameof(commandQuery));

            await PreProcessAsync(commandQuery, token).ConfigureAwait(false);

            Type commnadQueryType = commandQuery.GetType();
            Type type = typeof(ICommandQueryHandler<,>).MakeGenericType(commnadQueryType, typeof(TResult));
            dynamic instance = _getInstanceCallback(type);
            if (instance == null)
            {
                throw new TypeLoadException(
                    $"No command query handler type found for query command type: {commnadQueryType.FullName}");
            }

            dynamic specificCommandQuery = Convert.ChangeType(commandQuery, commnadQueryType);
            var result = await instance.ExecuteAsync(specificCommandQuery, token).ConfigureAwait(false);
            await PostProcessAsync(commandQuery, result, token).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Executes the query handler for the given query automatically.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the query.</returns>
        public async Task<IResult<TResult>> ExecuteAsync<TResult>(
            IQuery<TResult> query,
            CancellationToken token = default)
        {
            Guard.IsNotNull(query, nameof(query));

            await PreProcessAsync(query, token).ConfigureAwait(false);

            Type queryType = query.GetType();
            Type type = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));
            dynamic instance = _getInstanceCallback(type);
            if (instance == null)
            {
                throw new TypeLoadException(
                    $"No query handler type found for query type: {queryType.FullName}");
            }

            dynamic specificQuery = Convert.ChangeType(query, queryType);
            var result = await instance.ExecuteAsync(specificQuery, token).ConfigureAwait(false);
            await PostProcessAsync(query, result, token).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Allows handling of the input before any command has run.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="result">The result of the command.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        protected virtual Task PreProcessAsync(
            ICommand command,
            CancellationToken token = default) =>
            Task.CompletedTask;

        /// <summary>
        /// Allows handling of result after any command has run.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="result">The result of the command.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        protected virtual Task PostProcessAsync(
            ICommand command,
            IResult result,
            CancellationToken token = default) =>
            Task.CompletedTask;

        /// <summary>
        /// Allows handling of the input before any command query has run.
        /// </summary>
        /// <param name="commandQuery">The command query to execute.</param>
        /// <param name="result">The result of the command query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        protected virtual Task PreProcessAsync<TResult>(
            ICommandQuery<TResult> command,
            CancellationToken token = default) =>
            Task.CompletedTask;

        /// <summary>
        /// Allows handling of result after any command query has run.
        /// </summary>
        /// <param name="commandQuery">The command query to execute.</param>
        /// <param name="result">The result of the command query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        public virtual Task PostProcessAsync<TResult>(
            ICommandQuery<TResult> command,
            IResult<TResult> result,
            CancellationToken token = default) =>
            Task.CompletedTask;

        /// <summary>
        /// Allows handling of the input before any query has run.
        /// </summary>
        /// <param name="commandQuery">The query to execute.</param>
        /// <param name="result">The result of the query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        protected virtual Task PreProcessAsync<TResult>(
            IQuery<TResult> command,
            CancellationToken token = default) =>
            Task.CompletedTask;

        /// <summary>
        /// Allows handling of result after any query has run.
        /// </summary>
        /// <param name="commandQuery">The query to execute.</param>
        /// <param name="result">The result of the query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        public virtual Task PostProcessAsync<TResult>(
            IQuery<TResult> command,
            IResult<TResult> result,
            CancellationToken token = default) =>
            Task.CompletedTask;
    }
}