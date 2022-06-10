using FluentValidation;
using MetalCore.CQS.Sample.Core.DataStore;
using MetalCore.CQS.Sample.Core.Validation;
using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Core.Features.Cars.Commands.AddCar
{
    public class AddCarCommandValidation : FluentCommandValidatorBase<AddCarCommand>
    {
        private readonly ICarDataStore _dataStore;

        public AddCarCommandValidation(ICarDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        protected override void CreateRules(AddCarCommand input, CancellationToken token = default)
        {
            AddCarCommandValidator validator = new AddCarCommandValidator();

            //NOTE: Example of using Fluent Validation with the built in Validator.
            //      You don't have to use this class at all, you can always make your own.
            //      However, this has multi-threading and IoC setup built in and will
            //      also automatically run for all commands based on the inheritance.
            //
            // You could also just implement your checks directly here and not use any library.
            AddFluentRules<AddCarCommandValidator>(input, token);

            //NOTE: You can do async or sync here, up to you
            AddRule(ThreadRuleRunTypeEnum.Single, async () =>
            {
                List<BrokenRule> brokenRules = new List<BrokenRule>();
                if (_dataStore.Cars.Any(a => a.Make == input.Make && a.Model == input.Model && a.Year == input.Year))
                {
                    brokenRules.Add(new BrokenRule { RuleMessage = "Car already exists. " });
                }

                return await Task.FromResult(brokenRules);
            });
        }

        private class AddCarCommandValidator : AbstractValidator<AddCarCommand>
        {
            public AddCarCommandValidator()
            {
                RuleFor(a => a.Make).NotEmpty().NotNull();
                RuleFor(a => a.Model).NotEmpty().NotNull();
                RuleFor(a => a.Year).GreaterThanOrEqualTo(2000);
            }
        }
    }
}