namespace MetalCore.CQS.Mapper
{
    /// <summary>
    /// This interface defines how to map between two objects.
    /// </summary>
    /// <typeparam name="TFrom">The source of the mapping.</typeparam>
    /// <typeparam name="TTo">The target of the mapping.</typeparam>
    public interface IMapper<in TFrom, TTo>
    {
        /// <summary>
        /// Returns a mapped type.
        /// </summary>
        /// <param name="from">The source data.</param>
        /// <param name="to">The initial data to map to.</param>
        /// <returns>The target data.</returns>
        TTo Map(TFrom from, TTo to = default);
    }
}