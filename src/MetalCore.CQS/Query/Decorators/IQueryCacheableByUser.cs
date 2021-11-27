namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This interface marks a query to be cached automatically for a specific user.
    /// </summary>
    public interface IQueryCacheableByUser : IQueryCacheable
    {
    }
}