using System.Text;
using FluentValidation.Results;

namespace Wanderlust.Application.Common.Exceptions;

public class RequestValidationException<T> : Exception
{
    private readonly IList<ValidationFailure> _validationFailures;

    public RequestValidationException(IList<ValidationFailure> validationFailures)
    {
        _validationFailures = validationFailures
            ?? throw new ArgumentNullException(nameof(validationFailures));
    }

    public override string Message
    {
        get
        {
            var sb = new StringBuilder()
                .Append("Failed to validate type ")
                .Append(typeof(T).Name)
                .Append(": ");

            sb.AppendJoin(" ", _validationFailures.Select(f => f.ErrorMessage));

            return sb.ToString();
        }
    }
}