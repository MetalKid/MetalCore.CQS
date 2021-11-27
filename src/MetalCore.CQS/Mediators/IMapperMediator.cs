namespace MetalCore.CQS.Mediators
{
    /// <summary>
    /// This interface defines calling the correct mapper automatically.
    /// </summary>
    public interface IMapperMediator
    {
        /// <summary>
        /// Returns a mapped type.
        /// </summary>
        /// <typeparam name="TTo">The type of object being returned.</typeparam>
        /// <param name="from">The source data.</param>
        /// <param name="to">The initial data to map to.</param>
        /// <returns>The target data.</returns>
        TTo Map<TTo>(object from, TTo to = default);
    }
}