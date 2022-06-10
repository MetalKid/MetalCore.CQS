using FluentValidation;
using MetalCore.CQS.Command;
using MetalCore.CQS.Validation;
using System.Linq;
using System.Threading;

namespace MetalCore.CQS.Sample.Core.Validation
{
    public abstract class FluentCommandValidatorBase<TCommand> : CommandValidatorBase<TCommand>
        where TCommand : ICommand
    {
        protected void AddFluentRules<TValidator>(TCommand input, CancellationToken token = default)
            where TValidator : AbstractValidator<TCommand>, new()
        {
            var validator = new TValidator();

            AddRules(async () =>
            {
                FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(input, token);
                if (result.IsValid)
                {
                    return null;
                }

                return result.Errors.Select(a => new BrokenRule { RuleMessage = a.ErrorMessage, Relation = a.PropertyName });
            });
        }
    }
}
