using MetalCore.CQS.Command;
using MetalCore.CQS.Common;

namespace MetalCore.CQS.Sample.Core.Features.Cars.Commands.DeleteLastCar
{
    public class DeleteLastCarCommand : ICommand, ICqsRetry
    {
        public int MaxRetries => 2;
        public int RetryDelayMilliseconds => 100;
    }
}
