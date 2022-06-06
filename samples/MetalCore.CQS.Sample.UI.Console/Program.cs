namespace MetalCore.CQS.Sample.UI.Console
{
    using System;
    using MetalCore.CQS.Sample.UI.Console.IoC;
    using SimpleInjector;
    using System.Threading.Tasks;

    public class Program
    {
        private static readonly Container _container;

        static Program()
        {
            _container = IoCSetup.SetupIoC();
        }

        public static async Task Main()
        {
            ShowStartMessage();
            ShowMenu();

            var letter = Console.ReadKey();
            while (letter.KeyChar != 'Q' && letter.KeyChar != 'q')
            {
                await HandleKeyAsync(letter.KeyChar);
                letter = Console.ReadKey();
            }
        }

        private static void ShowStartMessage()
        {
            Console.WriteLine("Hello! Welcome to the CQS Demo! This will simulate queries/commands for you to debug through.");
            Console.WriteLine("");
        }

        private static void ShowMenu()
        {
            Console.WriteLine("--- Menu ---");
            Console.WriteLine("1. Query list of cars");
            Console.WriteLine("2. Query list of cars, filter for Chevy");
            Console.WriteLine("3. Add new random car");
            Console.WriteLine("4. Add existing car");
            Console.WriteLine("5. Delete last car, except the first 3");
            Console.WriteLine("6. Update the last car's model");
            Console.WriteLine("7. Add new car with a broken rule");
            Console.WriteLine("8. Add new car with no permission");
            Console.WriteLine("M to show Menu");
            Console.WriteLine("Q to Quit");
            Console.WriteLine("--- End ---");
            Console.WriteLine("");
        }

        private static async Task HandleKeyAsync(char letter)
        {
            Console.WriteLine("");

            switch (letter)
            {
                case 'm':
                case 'M':
                    ShowMenu();
                    break;
                case '1':
                    await SampleHelper.QueryListOfCarsAsync(_container);
                    return;
                case '2':
                    await SampleHelper.QueryListOfChevyCarsAsync(_container);
                    return;
                case '3':
                    await SampleHelper.AddNewRandomCarAsync(_container);
                    return;
                case '4':
                    await SampleHelper.AddExistingCarAsync(_container);
                    return;
                case '5':
                    await SampleHelper.DeleteLastCarAsync(_container);
                    return;
                case '6':
                    await SampleHelper.UpdateLastCarModelAsync(_container);
                    return;
                case '7':
                    await SampleHelper.AddCarWithBrokenRuleAsync(_container);
                    return;
                case '8':
                    await SampleHelper.AddCarWithNoPermissionAsync(_container);
                    return;
            }
        }
    }
}
