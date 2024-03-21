using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApi.Infraestrutura;

namespace WebApi.CalculoCdb;

public class CalculoCdbHandler(
    CalculoCdbCommandValidator validator,
    ISqlDataContext dataContext) : IRequestHandler<CalculoCdbCommand, CalculoCdbDto>
{
    private readonly CalculoCdbCommandValidator _validator = validator;
    private readonly ISqlDataContext _dataContext = dataContext;

    public Task<CalculoCdbDto> Handle(CalculoCdbCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationErrorException(validationResult.Errors);
        }

        var valorBruto = CalculeValorFinalDoCdb(request.ValorInicial, request.QuantidadeDeMeses);
        var valorLiquido = CalculoValorLiquidoDoCdb(valorBruto, request.ValorInicial, request.QuantidadeDeMeses);

        var resultadoDoCalculo = new CalculoCdbDto(valorBruto, valorLiquido);
        return Task.FromResult(resultadoDoCalculo);
    }

    private decimal CalculeValorFinalDoCdb(decimal valorInicial, int quantidadeDeMeses)
    {
        var cdb = _dataContext.Cdb
            .AsNoTracking()
            .OrderBy(e => e.UpdatedAt)
            .Last();

        decimal calculaCdb(decimal vi) => vi * (1 + (cdb.Cdi * cdb.TaxaBancaria));
        decimal valorFinal = calculaCdb(valorInicial);
        for (var i = 1; i < quantidadeDeMeses; i++)
        {
            valorFinal = calculaCdb(valorFinal);
        }

        return valorFinal;
    }

    private static decimal CalculoValorLiquidoDoCdb(decimal valorBruto, decimal valorInicial, int quantidadeDeMeses)
    {
        var aliquotaDoImposto = quantidadeDeMeses switch
        {
            > 0 and <= 6 => 0.225M,     // Alíquota do imposto de 22,5% para um prazo de até 06 meses.
            > 6 and <= 12 => 0.20M,     // Alíquota do imposto de 20% para um prazo de até 12 meses.
            > 12 and <= 24 => 0.175M,   // Alíquota do imposto de 17,5% para um prazo de até 24 meses.
            _ => 0.15M                  // Alíquota do imposto de 15% para um prazo acima de 24 meses.
        };

        var imposto = aliquotaDoImposto * (valorBruto - valorInicial);
        return valorBruto - imposto;
    }
}
