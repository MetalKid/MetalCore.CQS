using MetalCore.CQS.Command;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Features.Cars.Commands.AddCar
{
    public class AddCarCommandPermission : ICommandPermission<AddCarCommand>
    {
        //NOTE: You can IoC inject whatever you want to make this determination
        public async Task<bool> HasPermissionAsync(AddCarCommand command, CancellationToken token = default)
        {
            if (command.Year == 9999)
            {
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }
    }
}
