using MetalCore.CQS.Common;
using MetalCore.CQS.Query;
using System;
using System.Collections.Generic;

namespace MetalCore.CQS.Sample.Core.Features.Cars.Queries.ListOfCars
{
    /// <summary>
    /// This is the request of the query.
    /// </summary>
    public class ListOfCarsQuery : IQuery<ICollection<ListOfCarsDto>>, ICqsTimingWarningThreshold, IQueryCacheableAbsoluteTimespan
    {
        public string Make { get; set; }

        public int WarningThresholdMilliseconds => 10;

        public TimeSpan ExpireTimeout => TimeSpan.FromSeconds(10);
    }
}
