using FluentValidation.Results;

namespace WebApi.Infraestrutura;

public class ValidationError : Exception
{
    private readonly string _message;

    public ValidationError(IList<ValidationFailure> validationFailures)
    {
        var listaDeMensagensDeErro = validationFailures
            .Select(error => $"'{error.PropertyName}': {error.ErrorMessage};")
            .ToArray();
        _message = string.Join(Environment.NewLine, listaDeMensagensDeErro);
    }

    public override string Message => _message;
}
