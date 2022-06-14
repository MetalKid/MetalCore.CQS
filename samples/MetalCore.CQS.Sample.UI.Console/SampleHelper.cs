using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Mediators;
using MetalCore.CQS.Sample.Core.Features.Cars.CommandQueries.UpdateLastCarModel;
using MetalCore.CQS.Sample.Core.Features.Cars.Commands.AddCar;
using MetalCore.CQS.Sample.Core.Features.Cars.Commands.AddNewRandomCar;
using MetalCore.CQS.Sample.Core.Features.Cars.Commands.DeleteLastCar;
using MetalCore.CQS.Sample.Core.Features.Cars.Queries.ListOfCars;
using SimpleInjector;
namespace MetalCore.CQS.Sample.UI.Console
{
    using System;
    using SimpleInjector.Lifestyles;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class SampleHelper
    {
        public static async Task QueryListOfCarsAsync(Container container)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                ICqsMediator service = GetMediator(container);

                Console.WriteLine("--- 1. Query Result --");

                //NOTE: In ASP.NET MVC, you would inject ICqsMediator and then make this call directly
                IResult<ICollection<ListOfCarsDto>> result = await service.ExecuteAsync(new ListOfCarsQuery());

                foreach (ListOfCarsDto item in result.Data)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("--- End ---");
                Console.WriteLine("");
            }
        }

        public static async Task QueryListOfChevyCarsAsync(Container container)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                ICqsMediator service = GetMediator(container);

                Console.WriteLine("--- 2. Query Result --");

                //NOTE: In ASP.NET MVC, you would inject ICqsMediator and then make this call directly
                IResult<ICollection<ListOfCarsDto>> result = await service.ExecuteAsync(new ListOfCarsQuery { Make = "Chevy" });

                ShowResultData(result);

                foreach (ListOfCarsDto item in result.Data)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("--- End ---");
                Console.WriteLine("");
            }
        }

        public static async Task AddNewRandomCarAsync(Container container)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                ICqsMediator service = GetMediator(container);

                Console.WriteLine("--- 3. Command Result --");

                //NOTE: In ASP.NET MVC, you would inject ICqsMediator and then make this call directly
                IResult result = await service.ExecuteAsync(new AddNewRandomCarCommand());

                ShowResultData(result);

                Console.WriteLine("--- End ---");
                Console.WriteLine("");
            }
        }

        public static async Task AddExistingCarAsync(Container container)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                ICqsMediator service = GetMediator(container);

                Console.WriteLine("--- 4. Command Result --");

                //NOTE: In ASP.NET MVC, you would inject ICqsMediator and then make this call directly
                IResult result = await service.ExecuteAsync(new AddCarCommand { Make = "Chevy", Model = "Malibu", Year = 2022 });

                ShowResultData(result);

                Console.WriteLine("--- End ---");
                Console.WriteLine("");
            }
        }

        internal static async Task DeleteLastCarAsync(Container container)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                ICqsMediator service = GetMediator(container);

                Console.WriteLine("--- 5. Command Result --");

                //NOTE: In ASP.NET MVC, you would inject ICqsMediator and then make this call directly
                IResult result = await service.ExecuteAsync(new DeleteLastCarCommand());

                ShowResultData(result);

                Console.WriteLine("--- End ---");
                Console.WriteLine("");
            }
        }

        internal static async Task UpdateLastCarModelAsync(Container container)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                ICqsMediator service = GetMediator(container);

                Console.WriteLine("--- 6. Command Result --");

                //NOTE: In ASP.NET MVC, you would inject ICqsMediator and then make this call directly
                IResult<UpdateLastCarModelDto> result = await service.ExecuteAsync(new UpdateLastCarModelCommandQuery());

                Console.WriteLine($"New Model: {result.Data?.NewModel}");
                ShowResultData(result);

                Console.WriteLine("--- End ---");
                Console.WriteLine("");
            }
        }

        internal static async Task AddCarWithBrokenRuleAsync(Container container)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                ICqsMediator service = GetMediator(container);

                Console.WriteLine("--- 7. Command Result --");

                //NOTE: In ASP.NET MVC, you would inject ICqsMediator and then make this call directly
                IResult result = await service.ExecuteAsync(new AddCarCommand { Make = "", Model = null, Year = 999 });

                ShowResultData(result);

                Console.WriteLine("--- End ---");
                Console.WriteLine("");
            }
        }

        internal static async Task AddCarWithNoPermissionAsync(Container container)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                ICqsMediator service = GetMediator(container);

                Console.WriteLine("--- 8. Command Result --");

                //NOTE: In ASP.NET MVC, you would inject ICqsMediator and then make this call directly
                IResult result = await service.ExecuteAsync(new AddCarCommand { Make = "Chevy", Model = "Malibu", Year = 9999 });

                ShowResultData(result);

                Console.WriteLine("--- End ---");
                Console.WriteLine("");
            }
        }

        private static void ShowResultData(IResult result)
        {
            Console.WriteLine("Request was successful: " + result.IsSuccessful);

            if (result.BrokenRules?.Any() == true)
            {
                Console.WriteLine("Request had broken rules:");
                foreach (Validation.BrokenRule item in result.BrokenRules)
                {
                    Console.WriteLine($"{item.RuleMessage} {item.Relation}");
                }
            }

            if (result.HasDataNotFoundError)
            {
                Console.WriteLine("Request data not found.");
            }

            if (result.HasConcurrencyError)
            {
                Console.WriteLine("Someone else updated the request data first.");
            }

            if (result.HasNoPermissionError)
            {
                Console.WriteLine("Request did not have permission.");
            }

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                Console.WriteLine($"Unknown error: {result.ErrorMessage}");
            }
        }

        private static ICqsMediator GetMediator(Container container)
        {
            return container.GetInstance<ICqsMediator>();
        }
    }
}
