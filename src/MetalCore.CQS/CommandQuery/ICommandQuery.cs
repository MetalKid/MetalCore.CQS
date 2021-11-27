namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This interfaces defines a command that also returns a specific result type.
    /// </summary>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    // ReSharper disable once UnusedTypeParameter - Used by CommandQueryHandler
    public interface ICommandQuery<TResult>
    {
    }
}