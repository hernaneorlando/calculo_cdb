using FluentAssertions;
using System.Collections;
using WebApi.CalculoCdb;
using WebApi.Infraestrutura;

namespace WebApi.Test;

public class CalculoCdbHandlerTest
{
    /// <summary>
    /// Dado um valor monetário inicial inferior a 1(um)
    /// Quando o sitema calcular o CDB
    /// Então uma exceção do tipo <see cref="ValidationError"/> será lançada.
    /// </summary>
    /// <returns><see cref="ValidationError"/></returns>
    [Fact]
    public async Task RetornaExcecaoDevidoAValorInicialNaoPositivo()
    {
        // Cenário
        var random = new Random();
        var requisicao = new CalculoCdbCommand
        {
            ValorInicial = ObtenhaValorMonetarioInicialPositivo() * -1,
            QuantidadeDeMeses = 12
        };

        var validador = new CalculoCdbCommandValidator();
        var handler = new CalculoCdbHandler(validador);

        // Ação
        var acao = handler.Invoking(h => h.Handle(requisicao, CancellationToken.None));

        // Validação
        await acao.Should()
            .ThrowExactlyAsync<ValidationError>()
            .WithMessage($"'{nameof(CalculoCdbCommand.ValorInicial)}': O valor inicial deve ser maior que 0;");
    }

    /// <summary>
    /// Dado um prazo em meses menor ou igual a 1(um)
    /// Quando o sitema calcular o CDB
    /// Então uma exceção do tipo <see cref="ValidationError"/> será lançada.
    /// </summary>
    /// <returns><see cref="ValidationError"/></returns>
    [Fact]
    public async Task RetornaExcecaoDevidoAPrazoMensalMenorQueDois()
    {
        // Cenário
        var random = new Random();
        var requisicao = new CalculoCdbCommand
        {
            ValorInicial = ObtenhaValorMonetarioInicialPositivo(),
            QuantidadeDeMeses = random.Next(0, 1)
        };

        var validador = new CalculoCdbCommandValidator();
        var handler = new CalculoCdbHandler(validador);

        // Ação
        var acao = handler.Invoking(h => h.Handle(requisicao, CancellationToken.None));

        // Validação
        await acao.Should()
           .ThrowExactlyAsync<ValidationError>()
           .WithMessage($"'{nameof(CalculoCdbCommand.QuantidadeDeMeses)}': O prazo em meses deve ser maior que 1;");
    }

    /// <summary>
    /// Dado um valor monetário inicial positivo
    /// E um prazo em meses maior que 1(um)
    /// Quando o sitema calcular o CDB
    /// Então uma resposta do tipo <see cref="CalculoCdbDto"/> será retornada.
    /// </summary>
    /// <returns><see cref="CalculoCdbDto"/></returns>
    [Theory]
    [ClassData(typeof(ObtemAliquotaPorFaixaMensal))]
    public async Task RetornaComSucessoOResultadoDoCalculo(int meses, decimal aliquota)
    {
        // Cenário
        var requisicao = new CalculoCdbCommand
        {
            ValorInicial = ObtenhaValorMonetarioInicialPositivo(),
            QuantidadeDeMeses = meses
        };

        var validador = new CalculoCdbCommandValidator();
        var handler = new CalculoCdbHandler(validador);

        // Ação
        var calculoDto = await handler.Handle(requisicao, CancellationToken.None);

        // Validação
        calculoDto.Should().NotBeNull();
        calculoDto.ValorBruto.Should().BeGreaterThan(0);
        calculoDto.ValorLiquido.Should().BeLessThan(calculoDto.ValorBruto);
        
        var diferencaPercentual = 1 - (calculoDto.ValorLiquido / calculoDto.ValorBruto);
        diferencaPercentual.Should().Be(aliquota);
    }

    private static decimal ObtenhaValorMonetarioInicialPositivo()
    {
        var random = new Random();
        return (decimal)random.NextDouble() + random.Next(1000000000);
    }

    private class ObtemAliquotaPorFaixaMensal : IEnumerable<object[]>
    {
        private readonly Random random = new();

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]       // Alíquota do imposto de 22,5% para um prazo entre 02 e 06 meses.
            {
                random.Next(2, 7),
                0.225M
            };
            yield return new object[]       // Alíquota do imposto de 20% para um prazo de até 12 meses.
            {
                random.Next(7, 13),
                0.2M
            };
            yield return new object[]       // Alíquota do imposto de 17,5% para um prazo de até 24 meses.
            {
                random.Next(13, 25),
                0.175M
            };
            yield return new object[]       // Alíquota do imposto de 15% para um prazo acima de 24 meses.
            {                               // A fim de não estourar o valor máximo do tipo Decimal
                random.Next(25, 360),       // o total de meses foi limitado em 360 meses (30 anos).
                0.150M
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}