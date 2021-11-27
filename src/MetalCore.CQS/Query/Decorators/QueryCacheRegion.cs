using MetalCore.CQS.Validation;
using System;

namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This class generates the region key to use when caching an object.
    /// </summary>
    public abstract class QueryCacheRegion : IQueryCacheRegion
    {
        /// <summary>
        /// Returns the region (grouping) for the give type.
        /// </summary>
        /// <param name="queryType">The type of object to cache.</param>
        /// <returns>The region (grouping) to lookup/save the results.</returns>
        public string GetCacheRegion(Type queryType)
        {
            Guard.IsNotNull(queryType, nameof(queryType));

            string region = queryType.FullName;

            if (typeof(IQueryCacheableByUser).IsAssignableFrom(queryType))
            {
                region = $"{region}-{GetUserName() ?? string.Empty}";
            }
            if (typeof(IQueryCacheableByLanguage).IsAssignableFrom(queryType))
            {
                region = $"{region}-{GetLanguage() ?? string.Empty}";
            }

            return region;
        }

        /// <summary>
        /// Returns the currently logged in user's unique identifier.
        /// </summary>
        /// <returns>Currently logged in user's unique identifier.</returns>
        protected abstract string GetUserName();

        /// <summary>
        /// Returns the language of the current user.
        /// </summary>
        /// <returns>Language of the current user.</returns>
        protected abstract string GetLanguage();
    }
}