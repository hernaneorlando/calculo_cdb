using MediatR;

namespace WebApi.CalculoCdb;

public class CalculoCdbHandler(CalculoCdbCommandValidator validator) : IRequestHandler<CalculoCdbCommand, CalculoCdbDto>
{
    private readonly CalculoCdbCommandValidator _validator = validator;

    private const decimal CDI = 0.09M;      // Valor fixo do CDI em 0,9%
    private const decimal TB = 1.08M;       // Valor fixo da Taxa Bancária em 108%

    public Task<CalculoCdbDto> Handle(CalculoCdbCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationError(validationResult.Errors);
        }

        var valorBruto = CalculeValorFinalDoCdb(request.ValorInicial, request.QuantidaDeMeses);
        var valorLiquido = CalculoValorLiquidoDoCdb(valorBruto, request.QuantidaDeMeses);

        var resultadoDoCalculo = new CalculoCdbDto(valorBruto, valorLiquido);
        return Task.FromResult(resultadoDoCalculo);
    }

    private static decimal CalculeValorFinalDoCdb(decimal valorInicial, int quantidadeDeMeses)
    {
        //decimal valorFinalBruto = (decimal)(((double)valorInicial) * Math.Pow((double)(1 + (CDI * TB)), quantidadeDeMeses));
        var valorFinalBruto = 0M;
        for (var i = 1; i <= quantidadeDeMeses; i++)
        {
            valorFinalBruto = valorInicial * (1 + (CDI * TB));
            valorInicial = valorFinalBruto;
        }

        return valorFinalBruto;
    }

    private static decimal CalculoValorLiquidoDoCdb(decimal valorBruto, int quantidadeDeMeses)
    {
        var aliquotaDoImposto = quantidadeDeMeses switch
        {
            int meses when meses > 0 && meses <= 6      // Alíquota do imposto de 22,5% para um prazo de até 06 meses.
                => 0.225M,
            int meses when meses > 6 && meses <= 12     // Alíquota do imposto de 20% para um prazo de até 12 meses.
                => 0.20M,
            int meses when meses > 12 && meses <= 24    // Alíquota do imposto de 17,5% para um prazo de até 24 meses.
                => 0.175M,
            _ => 0.15M                                  // Alíquota do imposto de 15% para um prazo acima de 24 meses.
        };

        return valorBruto - (valorBruto * aliquotaDoImposto);
    }
}
