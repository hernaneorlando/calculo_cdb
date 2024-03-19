using FluentValidation;

namespace WebApi.CalculoCdb;

public class CalculoCdbCommandValidator : AbstractValidator<CalculoCdbCommand>
{
    public CalculoCdbCommandValidator()
    {
        RuleFor(calculo => calculo.ValorInicial)
            .GreaterThan(0)
            .WithMessage("O valor inicial deve ser maior que 0");

        RuleFor(calculo => calculo.QuantidadeDeMeses)
            .GreaterThan(1)
            .WithMessage("O prazo em meses deve ser maior que 1");
    }
}
